using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results
{
	public class WaitingMessageResult
	{
		/// <summary>
		/// The message that the user send.
		/// </summary>
		public SocketMessage Message { get; set; }
		/// <summary>
		/// Contains additional informations about the result.
		/// </summary>
		public Result Result { get; set; }
	}
}
