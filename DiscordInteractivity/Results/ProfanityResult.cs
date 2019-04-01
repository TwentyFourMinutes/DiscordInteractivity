using Discord.WebSocket;
using DiscordInteractivity.Core;
using DiscordInteractivity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordInteractivity.Results
{
	public class ProfanityResult
	{
		/// <summary>
		/// Gets the original Message which contains most likley profanity content.
		/// </summary>
		public SocketMessage Message { get; internal set; }
		/// <summary>
		/// Gets the profanity rating to the message content which is based on the <see cref="ProfanityIndicators"/> and <seealso cref="ProfanityWords"/>. Higher score indicates that it is more likley to be profanity content.
		/// </summary>
		public readonly double ProfanityRating;
		/// <summary>
		/// Gets the <see cref="ProfanityHandlerConfig.ProfanityIndicators"/> which got found in the message content;
		/// </summary>
		public readonly List<string> ProfanityIndicators;
		/// <summary>
		/// Gets the <see cref="ProfanityHandlerConfig.ProfanityWords"/> which are found in the message content. If the <seealso cref="ProfanityHandlerConfig.ProfanityOptions"/> are set to <seealso cref="ProfanityOptions.CheckForSimilarity"/> the Assumption is the detected word and the ProfanityWord is the original word from the <seealso cref="ProfanityHandlerConfig.ProfanityWords"/> Dictonary.
		/// </summary>
		public readonly List<(string ProfanityWord, string Assumption)> ProfanityWords;
		/// <summary>
		/// Determines what kind of Match the message content has.
		/// </summary>
		public readonly ProfanityMatch ProfanityMatch;

		internal ProfanityResult(double profanityRating, List<string> profanityIndicators, List<(string ProfanityWord, string Assumption)> badWords, ProfanityMatch match)
		{
			ProfanityRating = profanityRating;
			ProfanityIndicators = profanityIndicators;
			ProfanityWords = badWords;
			ProfanityMatch = match;
		}
	}
}
