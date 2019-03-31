# DiscordInteractivity

[![NuGet](https://img.shields.io/nuget/vpre/DiscordInteractivity.svg?style=plastic)](https://www.nuget.org/packages/DiscordInteractivity)

This is an addon for the Discord API Wrapper [Discord.Net](https://github.com/discord-net/Discord.Net) which adds some useful features as well as some interactivity.



## Features

- Waiting for a user message or a reaction in a specific channel

- Reply and delete messages/files with timeouts

- Send powerful Paginators which allow for custom paginated messages

- Set per user Cooldowns for individual commands and whole modules

- Profanity detection

  

## How to use it

Create a new instance of the `InteractiveService` e.g.
```cs
var _interactive = new InteractivityService(DiscordClient)
```

and add it to your ServiceCollection e.g.

```cs
public IServiceProvider BuildServiceProvider() => new ServiceCollection()
                    ...
                    .AddSingleton(_interactive)
                    .BuildServiceProvider();
```

For the last step all your Modules must inherit from `Ineractivity` if you want them to have the features this addons brings with it e.g.

```cs
public class Commands : Interactivity
{

...

}
```



## Note

- Some versions of this addon might be build against the [nightly builds](https://github.com/discord-net/Discord.Net#unstable-myget) of Discord.Net!

- Currently there is no Documentation, if you need any help regarding to the Addon feel free to send me a message on Discord, my Username is `24_minutes#7496`!

  

## Credits

- This package is inspired by [foxbot](https://github.com/foxbot) and his [Discord.Addons.Interactive](https://github.com/foxbot/Discord.Addons.Interactive) package.

- The [GetDistance](github.com/TwentyFourMinutes/DiscordInteractivity/blob/dev/DiscordInteractivity/Core/Profanity/ProfanityHandler.cs) method which implements the Damerau-Levenshtein algorithm. This [code](https://gist.github.com/wickedshimmy/449595) is written by [wickedshimmy](https://gist.github.com/wickedshimmy).

