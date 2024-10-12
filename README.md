# DiscordInteractivity

[![NuGet](https://img.shields.io/nuget/vpre/DiscordInteractivity.svg?style=plastic)](https://www.nuget.org/packages/DiscordInteractivity)

This is an addon for the Discord API Wrapper [Discord.Net](https://github.com/discord-net/Discord.Net) which adds a lot of useful features, which you can learn more about in the [documentation](https://github.com/TwentyFourMinutes/DiscordInteractivity/wik).

## Features

- Waiting for a user message or a reaction in a specific channel
- Reply and delete messages/files with timeouts
- Send powerful Paginators which allow for custom paginated messages
- Set per user Cooldowns for individual commands and whole modules
- Profanity detection
- Spam detection

## How to use it

Take a look at the [documentation](https://github.com/TwentyFourMinutes/DiscordInteractivity/wiki), which shows and explains many of the powerful features. For the really basic setup look [here](https://github.com/TwentyFourMinutes/DiscordInteractivity/wiki/InteractivityService#basic-setup) and follow the `Basic Setup` instructions.

If you need any further help regarding to the Addon feel free to send me a message on Discord, my Username is `24_minutes`!

## Credits

- This package is inspired by [foxbot](https://github.com/foxbot) and his [Discord.Addons.Interactive](https://github.com/foxbot/Discord.Addons.Interactive) package.
  - The [code](https://gist.github.com/wickedshimmy/449595) of the [GetDistance](https://github.com/TwentyFourMinutes/DiscordInteractivity/blob/dev/DiscordInteractivity/Core/Handlers/ProfanityHandler.cs) method which implements the Damerau-Levenshtein algorithm is written by [wickedshimmy](https://gist.github.com/wickedshimmy).