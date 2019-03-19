# DiscordInteractivity

This is an addon for the Discord API Wrapper [Discord.Net](https://github.com/discord-net/Discord.Net) which will add some useful features as well as some interactivity.



## Features

- Waiting for a User message in a specific Channel

- Reply and delete messages/files with timeouts

- Paginators which allow for paginated messages

  

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

This is a re-work of the [Discord.Addons.Net](https://github.com/foxbot/Discord.Addons.Interactive) addon by [foxbot](https://github.com/foxbot)!

