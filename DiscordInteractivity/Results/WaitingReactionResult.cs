using Discord.WebSocket;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Results
{
	/// <summary>
	/// The result of waiting on a reaction.
	/// </summary>
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
