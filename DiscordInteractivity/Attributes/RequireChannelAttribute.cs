using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInteractivity.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class RequireChannelAttribute : PreconditionAttribute
	{
		private readonly ulong[] _channelIds;

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
