using Discord.WebSocket;
using DiscordInteractivity.Configs;
using DiscordInteractivity.Core.Handlers;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results;

/// <summary>
/// This is the result of the profanity rating which contaisn information about the rating.
/// </summary>
public class ProfanityResult
{
    /// <summary>
    /// Gets the original Message which contains most likely profanity content.
    /// </summary>
    public SocketUserMessage Message { get; internal set; }

    /// <summary>
    /// Gets the profanity rating to the message content which is based on the <see cref="ProfanityIndicators"/> and <seealso cref="ProfanityWords"/>. Higher score indicates that it is more likley to be profanity content.
    /// </summary>
    public readonly double ProfanityRating;

    /// <summary>
    /// Gets the <see cref="ProfanityHandlerConfig.ProfanityIndicators"/> which got found in the message content;
    /// </summary>
    public readonly List<string> ProfanityIndicators;

    /// <summary>
    /// Gets the <see cref="ProfanityHandlerConfig.ProfanityWords"/> which are found in the message content and their <seealso cref="ProfanityMatch"/>."/>.
    /// </summary>
    public readonly List<ProfanityData> ProfanityWords;

    internal ProfanityResult(
        double profanityRating,
        List<string> profanityIndicators,
        List<ProfanityData> profanityWords
    )
    {
        ProfanityRating = profanityRating;
        ProfanityIndicators = profanityIndicators;
        ProfanityWords = profanityWords;
    }
}
