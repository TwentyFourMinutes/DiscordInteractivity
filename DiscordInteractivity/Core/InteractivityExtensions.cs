using Discord;
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
