using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordInteractivity.Core;
using DiscordInteractivity.Enums;
using System;
using System.Threading.Tasks;

namespace DiscordInteractivity.Pager
{
	public class Paginator : IDisposable
	{
		private PaginatorBuilder _paginator;
		private InteractivityService _interactivity;

		private readonly int _totalPages;
		private int _currentPage;

		private IUserMessage _message;

		public Paginator(PaginatorBuilder paginator)
		{
			_paginator = paginator;
			_totalPages = _paginator.Pages.Count;
		}

		internal async Task<IUserMessage> Initialize(InteractivityService interactivity, IMessageChannel channel, TimeSpan? timeOut)
		{
			_interactivity = interactivity;

			var page = GetCurrentPage();
			_message = await channel.SendMessageAsync(embed: page);

			_interactivity.DiscordClient.ReactionAdded += ReactionChanged;
			_interactivity.DiscordClient.ReactionRemoved += ReactionChanged;

			_ = Task.Delay(timeOut ?? interactivity.Config.DefaultPagerTimeout).ContinueWith(_ =>
			{
				_message.TryDeleteAsync().ConfigureAwait(false);
				this.Dispose();
			}).ConfigureAwait(false);

			AddReactions(_message);

			return _message;
		}

		private void AddReactions(IUserMessage msg)
		{
			_ = Task.Run(async () =>
			{
				try
				{
					await msg.AddReactionAsync(_interactivity.Config.StartEmoji);
					await msg.AddReactionAsync(_interactivity.Config.BacktEmoji);
					await msg.AddReactionAsync(_interactivity.Config.StopEmoji);
					await msg.AddReactionAsync(_interactivity.Config.ForwardEmoji);
					await msg.AddReactionAsync(_interactivity.Config.EndEmoji);
				}
				catch
				{
					//TODO
				}
			});
		}

		private async Task ReactionChanged(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
		{
			if (!arg1.HasValue || arg1.Value.Id != _message.Id || arg3.UserId != _paginator.Author.Id)
				return;

			var emoteName = arg3.Emote.Name;

			if (emoteName == _interactivity.Config.StartEmoji.Name)
				_currentPage = 0;
			else if (emoteName == _interactivity.Config.BacktEmoji.Name && _currentPage > 0)
				_currentPage--;
			else if (emoteName == _interactivity.Config.StopEmoji.Name)
			{
				await _message.TryDeleteAsync();
				Dispose();
				return;
			}
			else if (emoteName == _interactivity.Config.ForwardEmoji.Name && _currentPage + 1 < _totalPages)
				_currentPage++;
			else if (emoteName == _interactivity.Config.EndEmoji.Name)
				_currentPage = _totalPages - 1;
			else
				return;

			await _message.ModifyAsync(x => x.Embed = GetCurrentPage());
		}

		private Embed GetCurrentPage()
		{
			if (_currentPage > _totalPages - 1 || _currentPage < 0)
				throw new IndexOutOfRangeException("This Page does not exist!");

			var page = _paginator.Pages[_currentPage];

			EmbedBuilder eb = new EmbedBuilder
			{
				Color = page.Color ?? _paginator.Color,
				Description = page.Description,
				Title = page.Title ?? _paginator.Title,
				ImageUrl = page.ImageUrl ?? _paginator.ImageUrl,
				ThumbnailUrl = page.ThumbnailUrl ?? _paginator.ThumbnailUrl,
			};
			if (page.Fields != null)
				eb.WithFields(page.Fields);

			EmbedFooterBuilder efb = new EmbedFooterBuilder();

			if (_paginator.PaginatorFooter.HasFlag(PaginatorFooter.BotAuthor))
			{
				efb.IconUrl = _interactivity.BotOwner.GetAvatarUrl() ?? _interactivity.BotOwner.GetDefaultAvatarUrl();
				efb.Text = _interactivity.CopyrightInfo;
			}
			else if (_paginator.PaginatorFooter.HasFlag(PaginatorFooter.PaginatorAuthor))
			{
				efb.IconUrl = _paginator.Author.GetAvatarUrl() ?? _paginator.Author.GetDefaultAvatarUrl();
				efb.Text = $"{_paginator.Author.Username}#{_paginator.Author.Discriminator}";
			}
			if (_paginator.PaginatorFooter.HasFlag(PaginatorFooter.BotAuthor))
			{
				efb.Text += $" | Page {_currentPage + 1} out of {_totalPages}";
			}

			eb.WithFooter(efb);

			return eb.Build();
		}

		public void Dispose()
		{
			_interactivity.DiscordClient.ReactionAdded -= ReactionChanged;
			_interactivity.DiscordClient.ReactionRemoved -= ReactionChanged;
			_paginator = null;
		}
	}
}
