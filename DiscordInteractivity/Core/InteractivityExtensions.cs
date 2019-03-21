using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core
{
	public static class InteractivityExtensions
	{
		public static async Task<bool> TryDeleteAsync(this IDeletable deletable)
		{
			try
			{
				await deletable.DeleteAsync().ConfigureAwait(false);
			}
			catch { return false; }
			return true;
		}
	}
}
