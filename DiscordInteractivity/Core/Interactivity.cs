using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using DiscordInteractivity.Enums;
using DiscordInteractivity.Pager;
using DiscordInteractivity.Results;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core
{
	public class Interactivity : Interactivity<SocketCommandContext> { }

	public class Interactivity<T> : ModuleBase<T> where T : SocketCommandContext
	{
		public InteractivityService InteractivityService { get; set; }

		protected async Task<IUserMessage> ReplyAndDeleteAsync(string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendAndDeleteMessageAsync(text, isTTS, embed, timeOut, options).ConfigureAwait(false);

		protected async Task<IUserMessage> ReplyAndDeleteFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendAndDeleteFileAsync(filePath, text, isTTS, embed, timeOut, options).ConfigureAwait(false);

		protected async Task<IUserMessage> ReplyAndDeleteFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendAndDeleteFileAsync(stream, filename, text, isTTS, embed, timeOut, options).ConfigureAwait(false);

		protected async Task<IUserMessage> ReplyPaginatorAsync(Paginator paginator, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendPaginatorAsync(paginator, timeOut, options).ConfigureAwait(false);

		protected async Task<WaitingMessageResult> WaitForMessageAsync(IUser user, bool ignoreCommands = true, TimeSpan? timeOut = null)
			=> await Context.Channel.WaitForMessageAsync(user, ignoreCommands, timeOut).ConfigureAwait(false);

		protected async Task<WaitingReactionResult> WaitForReactionAsync(IUser user, TimeSpan? timeOut = null)
			=> await Context.Channel.WaitForReactionAsync(user, timeOut).ConfigureAwait(false);

		protected TimeSpan GetUptime() => InteractivityService.GetUptime();
		protected IUser GetBotAuthor() => InteractivityService.GetBotAuthor();
		protected string GetCopyrightInfo() => InteractivityService.GetCopyrightInfo();
	}
}
