using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results;

/// <summary>
/// The result of waiting on a reaction.
/// </summary>
public class WaitingReactionResult
{
    /// <summary>
    /// The reaction that the user added.
    /// </summary>
    public required SocketReaction? Message { get; init; }

    /// <summary>
    /// Contains additional information about the result.
    /// </summary>
    public required Result Result { get; init; }
}
