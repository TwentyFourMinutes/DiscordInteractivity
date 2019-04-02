using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Attributes;
using DiscordInteractivity.Callbacks;
using DiscordInteractivity.Configs;
using DiscordInteractivity.Core.Handlers;
using DiscordInteractivity.Enums;
using DiscordInteractivity.Results;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core
{
	/// <summary>
	/// The core class of DiscordInteractivity which is needed in order to enable most of the features.
	/// </summary>
	public class InteractivityService : IDisposable
	{
		internal readonly InteractivityServiceConfig Config;

		internal readonly DiscordSocketClient DiscordClient;
		internal readonly DiscordClientCallbacks DiscordCallbacks;
		internal readonly ProfanityHandler ProfanityHandler;
		internal readonly SpamHandler SpamHandler;

		internal readonly Timer ClearingTimer;

		/// <summary>
		/// This event gets fired when the ProfanityFilter detected any profanity message content.
		/// </summary>
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

		public event Func<SocketGuildUser, List<SocketUserMessage>, Task> SpamDetected
		{
			add
			{
				SpamHandler.SpamDetected += value;
			}
			remove
			{
				SpamHandler.SpamDetected -= value;
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
		/// Determines whether this instance is already Disposed or not.
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
			if (Config.SpamDetection)
				SpamHandler = new SpamHandler(this);

			ClearingTimer = new Timer(_ =>
			{
				Task.Run(() =>
				{
					foreach (var attribute in CooldownAttribute.CooldownAttributes)
					{
						if (attribute.IsToBeCleared)
							foreach (var cooldown in attribute.Cooldowns)
							{
								if (cooldown.Value.NextReset <= DateTime.UtcNow)
									attribute.Cooldowns.TryRemove(cooldown.Key, out TimeoutData _);
							}
					}
					if (SpamHandler != null)
					{
						foreach (var spaminfo in SpamHandler.SpamInformation)
						{
							if (spaminfo.Value.SpamReset <= DateTime.UtcNow)
							{
								SpamHandler.SpamInformation.TryRemove(spaminfo.Key, out var _);
							}
						}
					}
				});
			}, null, 600000, 600000);
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

		/// <summary>
		/// Gets a ProfanityResult from the data provided in the <see cref="ProfanityHandlerConfig"/>.
		/// </summary>
		/// <param name="content">The content to be rated.</param>
		/// <param name="options">Additional options which specify the way the rating works.</param>
		/// <returns></returns>
		public ProfanityResult GetProfanityRating(string content, ProfanityOptions options = ProfanityOptions.Default)
			=> ProfanityHandler.GetProfanityRating(content, options);

		/// <summary>
		/// Disposes all members and unsubscribes from all events.
		/// </summary>
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
