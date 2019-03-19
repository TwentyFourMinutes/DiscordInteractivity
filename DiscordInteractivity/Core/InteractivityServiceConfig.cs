using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace DiscordInteractivity.Core
{
	public class InteractivityServiceConfig
	{
		public DiscordSocketClient DiscordClient { get; set; }

		public List<string> CommandPrefixes { get; set; } = new List<string> { "!" };
		public bool HasMentionPrefix { get; set; } = true;

		public Emoji StartEmoji { get; set; } = new Emoji("⏮");
		public Emoji BacktEmoji { get; set; } = new Emoji("◀");
		public Emoji StopEmoji { get; set; } = new Emoji("⏹");
		public Emoji ForwardEmoji { get; set; } = new Emoji("▶");
		public Emoji EndEmoji { get; set; } = new Emoji("⏭");

		public TimeSpan DefaultMessageTimeout { get; set; } = TimeSpan.FromSeconds(15);
		public TimeSpan DefaultWaitingTimeout { get; set; } = TimeSpan.FromSeconds(30);
		public TimeSpan DefaultPagerTimeout { get; set; } = TimeSpan.FromSeconds(30);
	}
}
