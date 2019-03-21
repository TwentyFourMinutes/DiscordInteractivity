using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results
{
	public class WaitingMessageResult
	{
		public SocketMessage Message { get; set; }
		public Result Result { get; set; }
	}
}
