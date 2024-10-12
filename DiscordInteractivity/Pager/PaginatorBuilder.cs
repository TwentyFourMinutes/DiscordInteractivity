using System;
using System.Collections.Generic;
using Discord;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Pager
{
    /// <summary>
    /// The <see cref="PaginatorBuilder"/> is used to create paginated messages.
    /// </summary>
    public class PaginatorBuilder
    {
        /// <summary>
        /// Gets or sets the Pages which are used by the <see cref="Paginator"/>.
        /// </summary>
        public List<Page> Pages { get; set; }

        /// <summary>
        /// Gets or sets the default Embed color for all Pages.
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// Gets or sets the default Embed title for all Pages.
        /// </summary>
        public string Title { get; set; }

        internal IUser Author { get; set; }

        /// <summary>
        /// Gets or sets the default Embed thumbnailurl for all Pages.
        /// </summary>
        public string ThumbnailUrl { get; set; }
        /// <summary>
        /// Gets or sets the default Embed imageUrl for all Pages.
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// Determines whether other reactions should be removed from the <see cref="Paginator"/> or not.
        /// </summary>
        public bool RemoveOtherReactions { get; set; } = true;
        /// <summary>
        /// Gets or sets the footer of the <see cref="Paginator"/>.
        /// </summary>
        public PaginatorFooter PaginatorFooter { get; set; } = PaginatorFooter.PageNumber;

        /// <summary>
        /// Creates a new PaginatorBuilder which is linked to an <see cref="IUser"/>.
        /// </summary>
        /// <param name="author">This is the User which this instance of the PaginatorBuilder is linked to.</param>
        public PaginatorBuilder(IUser author)
        {
            Author = author;
            if (Author is null)
            {
                throw new ArgumentNullException("The Bot Author needs to be set!");
            }
        }

        /// <summary>
        /// Adds a page to the <see cref="Pages"/>.
        /// </summary>
        public void AddPage(Page page)
        {
            Pages.Add(page);
        }
        /// <summary>
        /// Adds muliple pages to the <see cref="Pages"/>.
        /// </summary>
        public void AddPages(List<Page> pages)
        {
            Pages.AddRange(pages);
        }

        /// <summary>
        /// Build the <see cref="PaginatorBuilder"/> to get a <see cref="Paginator"/> instance.
        /// </summary>
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
