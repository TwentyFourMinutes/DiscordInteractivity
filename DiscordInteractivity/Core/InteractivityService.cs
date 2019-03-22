using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Callbacks;
using System;

namespace DiscordInteractivity.Core
{
	public class InteractivityService : IDisposable
	{
		internal readonly InteractivityServiceConfig Config;
		internal readonly DiscordSocketClient DiscordClient;
		internal readonly DiscordClientCallbacks DiscordCallbacks;

		internal readonly DateTime StartupTime;
		internal IUser BotOwner;
		internal string CopyrightInfo
		{
			get
			{
				if (BotOwner is null)
					throw new InvalidOperationException("The BotOwner is not set yet.");
				return $"Bot made by {BotOwner.Username}#{BotOwner.Discriminator} {DateTime.Now.Year} ©";
			}
		}

		public bool IsDisposed { get; private set; }

		public InteractivityService(DiscordSocketClient discordClient) : this(new InteractivityServiceConfig { DiscordClient = discordClient }) { }
		public InteractivityService(InteractivityServiceConfig config)
		{
			if (config.DiscordClient is null)
				throw new ArgumentNullException("The DiscordClient can not be null!");

			Config = config;
			DiscordClient = Config.DiscordClient;
			StartupTime = DateTime.UtcNow;

			DiscordCallbacks = new DiscordClientCallbacks(this);

			DiscordClient.Ready += DiscordCallbacks.Ready;

			if (Config.SetExtensionReferenceAutomatically)
				InteractivityExtensions.SetInteractivityInstance(this);
		}

		public TimeSpan GetUptime() => DateTime.UtcNow - StartupTime;
		public IUser GetBotAuthor() => BotOwner;
		public string GetCopyrightInfo() => CopyrightInfo;

		public void Dispose()
		{
			if (!IsDisposed)
			{
				Config.DiscordClient.Ready -= DiscordCallbacks.Ready;
				IsDisposed = true;
			}
		}
	}
}
