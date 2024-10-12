using DiscordInteractivity.Core.Interactivity;

namespace DiscordInteractivity.Callbacks;

internal class DiscordClientCallbacks(InteractivityService service)
{
    internal async Task Ready()
    {
        var info = await service.DiscordClient.GetApplicationInfoAsync();
        service.BotOwner = info.Owner;

        if (service.Config.HasMentionPrefix)
            service.Config.CommandPrefixes.Add($"<@{service.DiscordClient.CurrentUser.Id}>");
    }
}
