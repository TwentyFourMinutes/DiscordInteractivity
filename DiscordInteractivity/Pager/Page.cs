using Discord;
using System.Collections.Generic;

namespace DiscordInteractivity.Pager
{
	/// <summary>
	/// This represents a page of a <see cref="PaginatorBuilder"/>.
	/// </summary>
	public class Page
	{
		/// <summary>
		/// Gets or sets the title of this Page.
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the color of this Page.
		/// </summary>
		public Color? Color { get; set; }

		/// <summary>
		/// Gets or sets the description of this Page.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the thumbnail url of this Page.
		/// </summary>
		public string ThumbnailUrl { get; set; }

		/// <summary>
		/// Gets or sets the image url of this Page.
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Gets or sets the fields of this Page.
		/// </summary>
		public List<EmbedFieldBuilder> Fields { get; set; }
	}
}
