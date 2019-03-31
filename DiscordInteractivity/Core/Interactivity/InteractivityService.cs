using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Callbacks;
using DiscordInteractivity.Core.Profanity;
using DiscordInteractivity.Results;
using System;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core
{
	public class InteractivityService : IDisposable
	{
		internal readonly InteractivityServiceConfig Config;

		internal readonly DiscordSocketClient DiscordClient;
		internal readonly DiscordClientCallbacks DiscordCallbacks;
		internal readonly ProfanityHandler ProfanityHandler;

		public event Func<ProfanityResult, Task> ProfanityAlert
		{
			add
			{
				ProfanityHandler.ProfanityAlert += value;
			}
			remove
			{
				ProfanityHandler.ProfanityAlert -= value;
			}
		}

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

		/// <summary>
		/// Initializes the instance with the default <see cref="InteractivityServiceConfig"/> and a <seealso cref="DiscordSocketClient"/>.
		/// </summary>
		/// <param name="discordClient">The Bots <see cref="DiscordSocketClient"/>.</param>
		public InteractivityService(DiscordSocketClient discordClient) : this(new InteractivityServiceConfig { DiscordClient = discordClient }) { }
		/// <summary>
		/// Initializes the instance with a <see cref="InteractivityServiceConfig"/>.
		/// </summary>
		/// <param name="interactivityConfig">The config under which the <see cref="InteractivityService"/> should operate.</param>
		public InteractivityService(InteractivityServiceConfig interactivityConfig)
		{
			Config = interactivityConfig;
			DiscordClient = Config.DiscordClient;
			StartupTime = DateTime.UtcNow;
			Config.PagerEmojis = new Emoji[5] { interactivityConfig.StartEmoji, interactivityConfig.BacktEmoji, interactivityConfig.StopEmoji, interactivityConfig.ForwardEmoji, interactivityConfig.EndEmoji };

			DiscordCallbacks = new DiscordClientCallbacks(this);

			DiscordClient.Ready += DiscordCallbacks.Ready;

			if (Config.SetExtensionReferenceAutomatically)
				InteractivityExtensions.SetInteractivityInstance(this);
		}
		/// <summary>
		/// If this constructor is used it will automatically activate a Profanity Filter.
		/// </summary>
		/// <param name="interactivityConfig">The config under which the <see cref="InteractivityService"/> should operate.</param>
		/// <param name="profanityConfig">The config under which the Profanity Filter should operate.</param>
		public InteractivityService(InteractivityServiceConfig interactivityConfig, ProfanityHandlerConfig profanityConfig) : this(interactivityConfig)
		{
			ProfanityHandler = new ProfanityHandler(this, profanityConfig);
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
				ProfanityHandler.Dispose();
				IsDisposed = true;
			}
		}
	}
}
