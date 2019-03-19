using Discord;
using DiscordInteractivity.Enums;
using System;
using System.Collections.Generic;

namespace DiscordInteractivity.Pager
{
	public class PaginatorBuilder
	{
		public List<Page> Pages { get; set; }

		public Color? Color { get; set; }

		public string Title { get; set; }

		public IUser Author { get; set; }

		public string ThumbnailUrl { get; set; }

		public string ImageUrl { get; set; }

		public PaginatorFooter PaginatorFooter { get; set; } = PaginatorFooter.PageNumber;

		public PaginatorBuilder(IUser author)
		{
			Author = author;
			if (Author is null)
			{
				throw new ArgumentNullException("The Bot Author needs to be set!");
			}
		}

		public void AddPage(Page page)
		{
			Pages.Add(page);
		}
		public void AddPages(List<Page> pages)
		{
			Pages.AddRange(pages);
		}

		public Paginator Build()
		{
			if (PaginatorFooter.HasFlag(PaginatorFooter.PaginatorAuthor) && PaginatorFooter.HasFlag(PaginatorFooter.BotAuthor))
			{
				throw new InvalidOperationException("You can not have the Paginator to display the BotAuthor and PaginatorAuthor at once!");
			}
			else if (Pages.Count == 0)
			{
				throw new InvalidOperationException("Your Builder needs at least one page!");
			}
			return new Paginator(this);
		}
	}
}
