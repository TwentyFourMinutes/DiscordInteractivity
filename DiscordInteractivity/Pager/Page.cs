using Discord;
using System.Collections.Generic;

namespace DiscordInteractivity.Pager
{
	public class Page
	{
		public string Title { get; set; }

		public Color? Color { get; set; }

		public string Description { get; set; }

		public string ThumbnailUrl { get; set; }

		public string ImageUrl { get; set; }

		public List<EmbedFieldBuilder> Fields { get; set; }
	}
}
