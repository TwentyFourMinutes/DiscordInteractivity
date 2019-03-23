using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results
{
	public class WaitingReactionResult
	{
		/// <summary>
		/// The reaction that the user added.
		/// </summary>
		public SocketReaction Message { get; set; }
		/// <summary>
		/// Contains additional informations about the result.
		/// </summary>
		public Result Result { get; set; }
	}
}
