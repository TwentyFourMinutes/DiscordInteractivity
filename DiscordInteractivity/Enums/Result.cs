using DiscordInteractivity.Results;

namespace DiscordInteractivity.Enums;

/// <summary>
/// A result information of a <see cref="WaitingMessageResult"/> or a <seealso cref="WaitingReactionResult"/>.
/// </summary>
public enum Result
{
    /// <summary>
    /// Determines that the user did respond.
    /// </summary>
    UserResponded,

    /// <summary>
    /// Determines that the user did not respond.
    /// </summary>
    TimedOut,
}
