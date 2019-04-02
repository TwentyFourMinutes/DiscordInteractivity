using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInteractivity.Attributes
{
	/// <summary>
	/// Requires the command to be executed in a specific channel.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequireChannelAttribute : PreconditionAttribute
	{
		private readonly ulong[] _channelIds;

		/// <summary>
		/// Creates a new <see cref="RequireChannelAttribute"/> with the channel ids provided.
		/// </summary>
		/// <param name="channelIds">Channels where the command can be executed in.</param>
		public RequireChannelAttribute(params ulong[] channelIds)
		{
			_channelIds = channelIds;
		}

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (!_channelIds.Any(x => x == context.Channel.Id))
			{
				return Task.FromResult(PreconditionResult.FromError("Command executed in wrong channel."));
			}
			return Task.FromResult	(PreconditionResult.FromSuccess());
		}
	}
}
