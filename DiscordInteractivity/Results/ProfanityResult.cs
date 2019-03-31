using Discord.WebSocket;
using DiscordInteractivity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordInteractivity.Results
{
	public class ProfanityResult
	{
		public SocketMessage Message { get; internal set; }
		public readonly double ProfanityRating;
		public readonly List<string> ProfanityIndicators;
		public readonly List<(string ProfanityWord, string Assumption)> BadWords;
		public readonly ProfanityMatch ProfanityMatch;

		internal ProfanityResult(double profanityRating, List<string> profanityIndicators, List<(string ProfanityWord, string Assumption)> badWords, ProfanityMatch match)
		{
			ProfanityRating = profanityRating;
			ProfanityIndicators = profanityIndicators;
			BadWords = badWords;
			ProfanityMatch = match;
		}
	}
}
