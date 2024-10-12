using Discord.Commands;

namespace DiscordInteractivity.Attributes;

/// <summary>
/// Requires the command to be executed in a specific channel.
/// </summary>
/// <remarks>
/// Creates a new <see cref="RequireChannelAttribute"/> with the channel ids provided.
/// </remarks>
/// <param name="channelIds">Channels where the command can be executed in.</param>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false
)]
public class RequireChannelAttribute(params ulong[] channelIds) : PreconditionAttribute
{
    public override Task<PreconditionResult> CheckPermissionsAsync(
        ICommandContext context,
        CommandInfo command,
        IServiceProvider services
    )
    {
        var result = channelIds.Any(x => x == context.Channel.Id)
            ? PreconditionResult.FromSuccess()
            : PreconditionResult.FromError("Command executed in wrong channel.");

        return Task.FromResult(result);
    }
}
