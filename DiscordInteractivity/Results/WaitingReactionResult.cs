using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results
{
	public class WaitingReactionResult
	{
		public SocketReaction Message { get; set; }
		public Result Result { get; set; }
	}
}
