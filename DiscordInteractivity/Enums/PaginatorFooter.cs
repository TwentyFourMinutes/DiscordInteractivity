using DiscordInteractivity.Pager;
using System;

namespace DiscordInteractivity.Enums
{
	/// <summary>
	/// Specifies which contents should be displayed in the footer of a <see cref="PaginatorBuilder"/>.
	/// </summary>
	[Flags]
	public enum PaginatorFooter
	{
		/// <summary>
		/// Displays the author of the <see cref="PaginatorBuilder"/> in the footer.
		/// </summary>
		PaginatorAuthor = 1,
		/// <summary>
		/// Displays the author of the bot in the footer.
		/// </summary>
		BotAuthor = 2,
		/// <summary>
		/// Displays the current page number in the footer.
		/// </summary>
		PageNumber = 4
	}
}
