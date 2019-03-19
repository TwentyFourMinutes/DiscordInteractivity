using Discord.WebSocket;
using DiscordInteractivity.Core;
using System;
using System.Threading.Tasks;

namespace DiscordInteractivity.Callbacks
{
	internal class DiscordClientCallbacks
	{
		private readonly InteractivityService _service;

		internal DiscordClientCallbacks(InteractivityService service)
		{
			_service = service;
		}

		internal async Task Ready()
		{
			var info = await _service.DiscordClient.GetApplicationInfoAsync();
			_service.BotOwner = info.Owner;
		}
	}
}
