using Discord.WebSocket;
using DiscordInteractivity.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordInteractivity.Results
{
	public class WaitingResult
	{
		public SocketMessage Message { get; set; }
		public Result Result { get; set; }
	}
}
