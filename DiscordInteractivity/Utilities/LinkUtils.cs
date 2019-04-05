using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInteractivity.Utilities
{
	public static class LinkUtils
	{
		public static string GetWebsiteName(string url)
		{
			if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
			{
				uri = new UriBuilder(url).Uri;
			}
			return uri.Host;
		}

		public static async Task<bool> WebsiteExists(string url, int timeout = 3000)
		{
			HttpWebResponse response = null;
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = "HEAD";
			request.Timeout = timeout;


			try
			{
				response = (HttpWebResponse) await request.GetResponseAsync();
			}
			catch
			{
				return false;
			}
			finally
			{
				response?.Close();
			}
			return true;
		}
	}
}
