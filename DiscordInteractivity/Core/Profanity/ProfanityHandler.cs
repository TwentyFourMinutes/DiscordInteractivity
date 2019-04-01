using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Enums;
using DiscordInteractivity.Results;

namespace DiscordInteractivity.Core.Profanity
{
	internal class ProfanityHandler : IDisposable
	{
		internal readonly ProfanityHandlerConfig Config;
		private readonly InteractivityService _service;

		private static readonly char[] _splits = { ' ' };

		public bool IsDisposed { get; private set; }

		internal event Func<ProfanityResult, Task> ProfanityAlert;

		internal ProfanityHandler(InteractivityService service, ProfanityHandlerConfig config)
		{
			Config = config;
			_service = service;
			if (Config.ScanNewMessages)
			{
				_service.DiscordClient.MessageReceived += MessageAction;
				_service.DiscordClient.MessageUpdated += MessageUpdated;
			}
		}

		private Task MessageAction(SocketMessage arg)
		{
			if (arg.Author.Id == _service.DiscordClient.CurrentUser.Id || _service.Config.CommandPrefixes.Any(x => arg.Content.StartsWith(x)))
				return Task.CompletedTask;

			var result = GetProfanityRating(arg.Content, Config.ProfanityOptions);

			if (result.ProfanityRating > 0)
			{
				result.Message = arg;
				_ = ProfanityAlert?.Invoke(result);
			}

			return Task.CompletedTask;
		}
		private Task MessageUpdated(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
			=> MessageAction(arg2);

		internal ProfanityResult GetProfanityRating(string content, ProfanityOptions options = ProfanityOptions.Default)
		{
			content = RemoveCharactersFromOptions(content, options);

			List<string> ProfanityIndicators = Config.ProfanityIndicators.Where(x => content.Contains(x)).ToList();
			List<(string ProfanityWord, string Assumption)> ProfainityWords = new List<(string, string)>();

			double rating = 0;

			if (ProfanityIndicators.Count() > 0)
			{
				rating += Config.IndicatorAdditon;
			}

			var ignoreDuplicates = options.HasFlag(ProfanityOptions.IgnoreDuplicateAssumptions);

			if (options.HasFlag(ProfanityOptions.CheckForSimilarity))
			{
				if (!options.HasFlag(ProfanityOptions.CheckWithoutWhitespaces))
				{
					var words = content.Split(_splits, StringSplitOptions.RemoveEmptyEntries);

					foreach (var badword in Config.ProfanityWords)
					{
						foreach (var word in words)
						{
							if (ignoreDuplicates && ProfainityWords.Any(x => x.Assumption == word))
								continue;

							var distance = GetDistance(badword.Key, word);

							var partlyMatch = word.Contains(badword.Key);

							if (distance <= Config.WordDistance)
							{
								ProfainityWords.Add((badword.Key, word));

								rating += (distance == 0) ? badword.Value : GetPositive(badword.Value - 1);

								if (rating >= Config.TriggerOn && Config.TriggerOn != -1)
									return new ProfanityResult(rating, ProfanityIndicators, ProfainityWords, (distance == 0) ? ProfanityMatch.FullMatch : ProfanityMatch.SimilarMatch);
							}
							else if (partlyMatch)
							{
								ProfainityWords.Add((badword.Key, word));

								rating += GetPositive(badword.Value - 0.5);

								if (rating >= Config.TriggerOn && Config.TriggerOn != -1)
									return new ProfanityResult(rating, ProfanityIndicators, ProfainityWords, ProfanityMatch.PartlyMatch);
							}
						}
					}
				}
				else
				{
					foreach (var badword in Config.ProfanityWords)
					{
						for (int i = 0; i <= content.Length - badword.Key.Length; i++)
						{
							var word = content.Substring(i, badword.Key.Length);
							var distance = GetDistance(badword.Key, word);

							if (distance <= Config.WordDistance)
							{
								if (ignoreDuplicates && ProfainityWords.Any(x => x.Assumption == word))
									continue;

								ProfainityWords.Add((badword.Key, word));

								rating += GetPositive(badword.Value - 1);

								if (rating >= Config.TriggerOn && Config.TriggerOn != -1)
									return new ProfanityResult(rating, ProfanityIndicators, ProfainityWords, (distance == 0) ? ProfanityMatch.FullMatch : ProfanityMatch.PartlyMatch);
							}
						}
					}
				}
			}
			return new ProfanityResult(rating, ProfanityIndicators, ProfainityWords, ProfanityMatch.NoMatch);
		}
		private string RemoveCharactersFromOptions(string content, ProfanityOptions options)
		{
			StringBuilder sb = new StringBuilder();

			bool rNAC = options.HasFlag(ProfanityOptions.RemoveNoneAlphanumericCharcaters);
			bool cWW = options.HasFlag(ProfanityOptions.CheckWithoutWhitespaces);

			foreach (var character in content)
			{
				if (char.IsWhiteSpace(character))
				{
					if (cWW)
						continue;
					else
						sb.Append(character);
				}
				if (rNAC && !char.IsLetterOrDigit(character))
					continue;
				else
					sb.Append(character);
			}

			return sb.ToString().ToLower();
		}
		private int GetDistance(string original, string modified)
		{
			if (original == modified)
				return 0;

			int len_orig = original.Length;
			int len_diff = modified.Length;

			if (len_orig == 0)
				return len_diff;

			if (len_diff == 0)
				return len_orig;

			var matrix = new int[len_orig + 1, len_diff + 1];

			for (int i = 1; i <= len_orig; i++)
			{
				matrix[i, 0] = i;
				for (int j = 1; j <= len_diff; j++)
				{
					int cost = modified[j - 1] == original[i - 1] ? 0 : 1;
					if (i == 1)
						matrix[0, j] = j;

					var vals = new int[] {
					matrix[i - 1, j] + 1,
					matrix[i, j - 1] + 1,
					matrix[i - 1, j - 1] + cost
				};
					matrix[i, j] = vals.Min();
					if (i > 1 && j > 1 && original[i - 1] == modified[j - 2] && original[i - 2] == modified[j - 1])
						matrix[i, j] = Math.Min(matrix[i, j], matrix[i - 2, j - 2] + cost);
				}
			}
			return matrix[len_orig, len_diff];
		}
		private double GetPositive(double number)
		{
			return (number > 0) ? number : 0;
		}

		public void Dispose()
		{
			if (!IsDisposed)
			{
				if (Config.ScanNewMessages)
				{
					_service.DiscordClient.MessageReceived -= MessageAction;
					_service.DiscordClient.MessageUpdated -= MessageUpdated;
				}
				IsDisposed = true;
			}
		}
	}
}
