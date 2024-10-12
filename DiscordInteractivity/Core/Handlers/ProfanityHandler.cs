using System.Text;
using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Configs;
using DiscordInteractivity.Core.Interactivity;
using DiscordInteractivity.Enums;
using DiscordInteractivity.Results;

namespace DiscordInteractivity.Core.Handlers;

internal class ProfanityHandler : IDisposable
{
    internal readonly ProfanityHandlerConfig Config;
    private readonly InteractivityService _service;

    private static readonly char[] _splits = [' '];

    public bool IsDisposed { get; private set; }

    internal event Func<ProfanityResult, Task>? ProfanityAlert;

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
        if (
            arg.Author.Id == _service.DiscordClient.CurrentUser.Id
            || (Config.CheckCommands && _service.Config.CommandPrefixes.Any(arg.Content.StartsWith))
            || arg is not SocketUserMessage message
            || (
                _service.Config.IgnoreRolesPosition != -1
                && ((SocketGuildUser)message.Author).Roles.Max(x => x.Position)
                    < _service.Config.IgnoreRolesPosition
            )
        )
        {
            return Task.CompletedTask;
        }

        var result = GetProfanityRating(message.Content, Config.ProfanityOptions);

        if (result.ProfanityRating >= Config.TriggerOn)
        {
            result.Message = message;
            _ = ProfanityAlert?.Invoke(result);
        }

        return Task.CompletedTask;
    }

    private Task MessageUpdated(
        Cacheable<IMessage, ulong> arg1,
        SocketMessage arg2,
        ISocketMessageChannel arg3
    ) => MessageAction(arg2);

    internal ProfanityResult GetProfanityRating(
        string content,
        ProfanityOptions options = ProfanityOptions.Default
    )
    {
        content = RemoveCharactersFromOptions(content, options);

        var profanityIndicators = Config
            .ProfanityIndicators.Where(x => content.Contains(x))
            .ToList();
        var profanityWords = new List<ProfanityData>();

        var rating = 0d;

        if (profanityIndicators.Count() > 0)
        {
            rating += Config.IndicatorAddition;
        }

        var ignoreDuplicates = options.HasFlag(ProfanityOptions.IgnoreDuplicateAssumptions);

        var words = content.Split(_splits, StringSplitOptions.RemoveEmptyEntries);

        if (!options.HasFlag(ProfanityOptions.CheckWithoutWhitespaces))
        {
            foreach (var badWord in Config.ProfanityWords)
            {
                foreach (var word in words)
                {
                    if (ignoreDuplicates && profanityWords.Any(x => x.Assumption == word))
                        continue;

                    var distance = GetDistance(badWord.Key, word);

                    var partlyMatch = word.Contains(badWord.Key);

                    if (distance <= Config.WordDistance)
                    {
                        profanityWords.Add(
                            new ProfanityData
                            {
                                ProfanityWord = badWord.Key,
                                Assumption = word,
                                MatchType =
                                    (distance == 0)
                                        ? ProfanityMatch.FullMatch
                                        : ProfanityMatch.SimilarMatch,
                            }
                        );

                        rating += (distance == 0) ? badWord.Value : GetPositive(badWord.Value - 2);

                        if (rating >= Config.EarlyTriggerOn && Config.EarlyTriggerOn != -1)
                            return new ProfanityResult(rating, profanityIndicators, profanityWords);
                    }
                    else if (partlyMatch)
                    {
                        profanityWords.Add(
                            new ProfanityData
                            {
                                ProfanityWord = badWord.Key,
                                Assumption = word,
                                MatchType = ProfanityMatch.PartlyMatch,
                            }
                        );

                        rating += GetPositive(badWord.Value - 1);

                        if (rating >= Config.EarlyTriggerOn && Config.EarlyTriggerOn != -1)
                            return new ProfanityResult(rating, profanityIndicators, profanityWords);
                    }
                }
            }
        }
        else
        {
            foreach (var badWord in Config.ProfanityWords)
            {
                for (int i = 0; i <= content.Length - badWord.Key.Length; i++)
                {
                    var word = content.Substring(i, badWord.Key.Length);
                    var distance = GetDistance(badWord.Key, word);

                    if (distance <= Config.WordDistance)
                    {
                        if (ignoreDuplicates && profanityWords.Any(x => x.Assumption == word))
                            continue;

                        profanityWords.Add(
                            new ProfanityData
                            {
                                ProfanityWord = badWord.Key,
                                Assumption = word,
                                MatchType = ProfanityMatch.SimilarMatch,
                            }
                        );

                        rating += GetPositive(badWord.Value - 1);

                        if (rating >= Config.EarlyTriggerOn && Config.EarlyTriggerOn != -1)
                            return new ProfanityResult(rating, profanityIndicators, profanityWords);
                    }
                }
            }
        }

        if (words.Length == 1)
        {
            rating +=
                (profanityWords.FirstOrDefault()?.MatchType == ProfanityMatch.FullMatch) ? 1 : 0.5;
        }

        return new ProfanityResult(rating, profanityIndicators, profanityWords);
    }

    private string RemoveCharactersFromOptions(string content, ProfanityOptions options)
    {
        var sb = new StringBuilder();

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

                var vals = new int[]
                {
                    matrix[i - 1, j] + 1,
                    matrix[i, j - 1] + 1,
                    matrix[i - 1, j - 1] + cost,
                };
                matrix[i, j] = vals.Min();
                if (
                    i > 1
                    && j > 1
                    && original[i - 1] == modified[j - 2]
                    && original[i - 2] == modified[j - 1]
                )
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

public class ProfanityData
{
    public required string ProfanityWord { get; init; }
    public required string Assumption { get; init; }
    public required ProfanityMatch MatchType { get; init; }
}
