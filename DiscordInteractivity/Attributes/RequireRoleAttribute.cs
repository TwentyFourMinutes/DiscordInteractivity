using Discord.Commands;
using Discord.WebSocket;

namespace DiscordInteractivity.Attributes;

/// <summary>
/// Requires a user to match these roles in order to execute a command.
/// </summary>
/// <remarks>
/// Creates a new <see cref="RequireRoleAttribute"/> with the role ids provided.
/// </remarks>
/// <param name="roleIds">The roles which the user needs to match.</param>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false
)]
public class RequireRoleAttribute(params ulong[] roleIds) : PreconditionAttribute
{
    /// <summary>
    /// Determines whether all role ids provided must match the user or not.
    /// </summary>
    public bool MatchAllRoles = false;

    public override Task<PreconditionResult> CheckPermissionsAsync(
        ICommandContext context,
        CommandInfo command,
        IServiceProvider services
    )
    {
        PreconditionResult result;

        if (context.User is not SocketGuildUser user)
        {
            result = PreconditionResult.FromError("Command not invoked in a Guild.");
        }
        else if (!MatchAllRoles && roleIds.Any(x => user.Roles.Any(y => y.Id == x)))
        {
            result = PreconditionResult.FromSuccess();
        }
        else if (MatchAllRoles && roleIds.All(x => user.Roles.Any(y => y.Id == x)))
        {
            result = PreconditionResult.FromSuccess();
        }
        else
        {
            result = PreconditionResult.FromError("User doesn't match any roles.");
        }

        return Task.FromResult(result);
    }
}
