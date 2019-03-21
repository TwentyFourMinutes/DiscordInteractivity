using DiscordInteractivity.Core;
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

			if (_service.Config.HasMentionPrefix)
				_service.Config.CommandPrefixes.Add($"<@{_service.DiscordClient.CurrentUser.Id}>");
		}
	}
}
