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

		/// <summary>
		/// Determines whether this instance is already Dispoed or not.
		/// </summary>
		public bool IsDisposed { get; private set; }

		public InteractivityService(DiscordSocketClient discordClient) : this(new InteractivityServiceConfig { DiscordClient = discordClient }) { }
		public InteractivityService(InteractivityServiceConfig config)
		{
			if (config.DiscordClient is null)
				throw new ArgumentNullException("The DiscordClient can not be null!");

			Config = config;
			DiscordClient = Config.DiscordClient;
			StartupTime = DateTime.UtcNow;
			Config.PagerEmojis = new Emoji[5] { config.StartEmoji, config.BacktEmoji, config.StopEmoji, config.ForwardEmoji, config.EndEmoji };

			DiscordCallbacks = new DiscordClientCallbacks(this);

			DiscordClient.Ready += DiscordCallbacks.Ready;

			if (Config.SetExtensionReferenceAutomatically)
				InteractivityExtensions.SetInteractivityInstance(this);
		}

		/// <summary>
		/// Return the duration of the bot since the start of the Applications.
		/// </summary>
		public TimeSpan GetUptime() => DateTime.UtcNow - StartupTime;
		/// <summary>
		/// Return the author of the Bot author.
		/// </summary>
		public IUser GetBotAuthor() => BotOwner;
		/// <summary>
		/// Return the copyright info.
		/// </summary>
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
