using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results;

/// <summary>
/// The result of waiting on a message.
/// </summary>
public class WaitingMessageResult
{
    /// <summary>
    /// The message that the user send.
    /// </summary>
    public required SocketMessage? Message { get; init; }

    /// <summary>
    /// Contains additional information about the result.
    /// </summary>
    public required Result Result { get; init; }
}
