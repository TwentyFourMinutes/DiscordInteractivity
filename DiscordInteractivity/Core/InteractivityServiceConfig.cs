using Discord;
using Discord.WebSocket;
using System;

namespace DiscordInteractivity.Core
{
	public class InteractivityServiceConfig
	{
		public DiscordSocketClient DiscordClient { get; set; }

		public bool GenerateCopyrightInfo { get; set; } = false;

		public Emoji StartEmoji { get; set; } = new Emoji("⏮");
		public Emoji BacktEmoji { get; set; } = new Emoji("◀");
		public Emoji StopEmoji { get; set; } = new Emoji("⏹");
		public Emoji ForwardEmoji { get; set; } = new Emoji("▶");
		public Emoji EndEmoji { get; set; } = new Emoji("⏭");

		public TimeSpan DefaultMessageTimeout { get; set; } = TimeSpan.FromSeconds(15);
		public TimeSpan DefaultPagerTimeout { get; set; } = TimeSpan.FromSeconds(30);
	}
}
