using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Core;
using DiscordInteractivity.Enums;
using DiscordInteractivity.Pager;
using System;
using System.Collections.Generic;

namespace DiscordInteractivity.Configs
{
	/// <summary>
	/// The config for a interactivity service handler.
	/// </summary>
	public class InteractivityServiceConfig
	{
		/// <summary>
		/// Gets or sets the Discord client that the <see cref="InteractivityService"/> is going to use.
		/// </summary>
		public DiscordSocketClient DiscordClient { get; set; }

		/// <summary>
		/// Gets or sets the command prefixes that are going to be ignored by commands.
		/// </summary>
		public List<string> CommandPrefixes { get; set; } = new List<string> { "!" };
		/// <summary>
		/// Gets or sets whether the Bot is also listening on his mention as a prefix or not.
		/// </summary>
		public bool HasMentionPrefix { get; set; } = true;

		/// <summary>
		/// Gets or sets whether the <see cref="InteractivityExtensions"/> should automatically update the instance of the <see cref="InteractivityService"/> or not.
		/// </summary>
		public bool SetExtensionReferenceAutomatically { get; set; } = true;

		/// <summary>
		/// Gets or sets the <see cref="Emoji"/> that will be displayed on the <see cref="Paginator"/> in order to jump to the first page.
		/// </summary>
		public Emoji StartEmoji { get; set; } = new Emoji("⏮");
		/// <summary>
		/// Gets or sets the <see cref="Emoji"/> that will be displayed on the <see cref="Paginator"/> in order to go to the page before.
		/// </summary>
		public Emoji BacktEmoji { get; set; } = new Emoji("◀");
		/// <summary>
		/// Gets or sets the <see cref="Emoji"/> that will be displayed on the <see cref="Paginator"/> in order to stop the Paginator.
		/// </summary>
		public Emoji StopEmoji { get; set; } = new Emoji("⏹");
		/// <summary>
		/// Gets or sets the <see cref="Emoji"/> that will be displayed on the <see cref="Paginator"/> in order to go to the next page.
		/// </summary>
		public Emoji ForwardEmoji { get; set; } = new Emoji("▶");
		/// <summary>
		/// Gets or sets the <see cref="Emoji"/> that will be displayed on the <see cref="Paginator"/> in order to jump to the last page.
		/// </summary>
		public Emoji EndEmoji { get; set; } = new Emoji("⏭");

		internal Emoji[] PagerEmojis;

		/// <summary>
		/// Gets or sets the default timeout for messages that get deleted after they are sent.
		/// </summary>
		public TimeSpan DefaultMessageTimeout { get; set; } = TimeSpan.FromSeconds(15);
		/// <summary>
		/// Gets or sets the default timeout for waiting on users.
		/// </summary>
		public TimeSpan DefaultWaitingTimeout { get; set; } = TimeSpan.FromSeconds(30);
		/// <summary>
		/// Gets or sets the default timeout for Paginators that get deleted after they are sent.
		/// </summary>
		public TimeSpan DefaultPagerTimeout { get; set; } = TimeSpan.FromSeconds(30);

		/// <summary>
		/// Gets or sets the <see cref="TimeSpan"/> before the spam detection resets the <seealso cref="SpamCount"/> of a user.
		/// </summary>
		public TimeSpan SpamDuration { get; set; } = TimeSpan.FromSeconds(3);
		/// <summary>
		/// Gets or sets the amount of messages need to be send in the <see cref="SpamDuration"/> in order to trigger the spam detection.
		/// </summary>
		public int SpamCount { get; set; } = 5;
		/// <summary>
		/// Gets or sets whether the spam protection should be enabled or not.
		/// </summary>
		public bool SpamDetection { get; set; } = false;
	}
}
