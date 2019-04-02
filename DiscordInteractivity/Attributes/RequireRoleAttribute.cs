using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInteractivity.Attributes
{
	/// <summary>
	/// Requires a user to match these roles in order to execute a command.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequireRoleAttribute : PreconditionAttribute
	{
		private readonly ulong[] _roleIds;

		/// <summary>
		/// Determines whether all role ids provided must match the user or not.
		/// </summary>
		public bool MatchAllRoles = false;

		/// <summary>
		/// Creates a new <see cref="RequireRoleAttribute"/> with the role ids provided.
		/// </summary>
		/// <param name="roleIds">The roles which the user needs to match.</param>
		public RequireRoleAttribute(params ulong[] roleIds)
		{
			_roleIds = roleIds;
		}

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (!(context.User is SocketGuildUser user))
				return Task.FromResult(PreconditionResult.FromError("Command not invoked in a Guild."));

			if (!MatchAllRoles && _roleIds.Any(x => user.Roles.Any(y => y.Id == x)))
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			else if (MatchAllRoles && _roleIds.All(x => user.Roles.Any(y => y.Id == x)))
			{
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			return Task.FromResult(PreconditionResult.FromError("User doesn't match any roles."));
		}
	}
}
