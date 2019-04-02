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
	/// <summary>
	/// Inherit from this class to get all sort of useful features.
	/// </summary>
	public class Interactivity : Interactivity<SocketCommandContext> { }

	/// <summary>
	/// Inherit from this class to get all sort of useful features.
	/// </summary>
	public class Interactivity<T> : ModuleBase<T> where T : SocketCommandContext
	{
		/// <summary>
		/// Your <see cref="InteractivityService"/> instance you provided in the dependency injection.
		/// </summary>
		public InteractivityService InteractivityService { get; set; }

		/// <summary>
		/// Sends a message to current channel and deletes it after the <see cref="TimeSpan"/> elapsed.
		/// </summary>
		/// <param name="text">The message to be sent.</param>
		/// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
		/// <param name="embed">The <see cref="Embed"/> to be sent.</param>
		/// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the message.</param>
		/// <param name="options">The options to be used while sending the request.</param>
		/// <exception cref="InvalidOperationException"/>
		protected async Task<IUserMessage> ReplyAndDeleteAsync(string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendAndDeleteMessageAsync(text, isTTS, embed, timeOut, options).ConfigureAwait(false);
		/// <summary>
		/// Sends a file to the current channel with an optional caption and deletes it after the <see cref="TimeSpan"/> elapsed.
		/// </summary>
		/// <param name="filePath">The file path of the file to be sent.</param>		
		/// <param name="text">The message to be sent.</param>
		/// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
		/// <param name="embed">The <see cref="Embed"/> to be sent.</param>
		/// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the message.</param>
		/// <param name="options">The options to be used while sending the request.</param>
		/// <exception cref="InvalidOperationException"/>
		protected async Task<IUserMessage> ReplyAndDeleteFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendAndDeleteFileAsync(filePath, text, isTTS, embed, timeOut, options).ConfigureAwait(false);
		/// <summary>
		/// Sends a file to the current channel with an optional caption and deletes it after the <see cref="TimeSpan"/> elapsed.
		/// </summary>
		/// <param name="stream">The stream of the file to be sent.</param>	
		/// <param name="filename">The name of the attachment.</param>
		/// <param name="text">The message to be sent.</param>
		/// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
		/// <param name="embed">The <see cref="Embed"/> to be sent.</param>
		/// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the message.</param>
		/// <param name="options">The options to be used while sending the request.</param>
		/// <exception cref="InvalidOperationException"/>
		protected async Task<IUserMessage> ReplyAndDeleteFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendAndDeleteFileAsync(stream, filename, text, isTTS, embed, timeOut, options).ConfigureAwait(false);
		/// <summary>
		/// Sends a <see cref="Paginator"/> to the current channel and deletes it after the <see cref="TimeSpan"/> elapsed.
		/// </summary>	
		/// <param name="paginator">The <see cref="Paginator"/> to be sent.</param>
		/// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the <see cref="Paginator"/>.</param>
		/// <param name="options">The options to be used while sending the request.</param>
		/// <exception cref="InvalidOperationException"/>
		protected async Task<IUserMessage> ReplyPaginatorAsync(Paginator paginator, TimeSpan? timeOut = null, RequestOptions options = null)
			=> await Context.Channel.SendPaginatorAsync(paginator, timeOut, options).ConfigureAwait(false);
		/// <summary>
		/// Waits for an <see cref="IUser"/> to sent a message in a specific channel.
		/// </summary>
		/// <param name="user">The <see cref="IUser"/> to be waited for.</param>
		/// <param name="ignoreCommands">Determines whether messages with Command Prefixes should be ignored.</param>
		/// <param name="timeOut">The <see cref="TimeSpan"/> it waits for the user.</param>
		/// <exception cref="InvalidOperationException"/>
		protected async Task<WaitingMessageResult> WaitForMessageAsync(IUser user, bool ignoreCommands = true, TimeSpan? timeOut = null)
			=> await Context.Channel.WaitForMessageAsync(user, ignoreCommands, timeOut).ConfigureAwait(false);
		/// <summary>
		/// Waits for an <see cref="IUser"/> to react in a specific channel.
		/// </summary>
		/// <param name="user">The <see cref="IUser"/> to be waited for.</param>
		/// <param name="timeOut">The <see cref="TimeSpan"/> it waits for the user.</param>
		/// <exception cref="InvalidOperationException"/>
		protected async Task<WaitingReactionResult> WaitForReactionAsync(IUser user, TimeSpan? timeOut = null)
			=> await Context.Channel.WaitForReactionAsync(user, timeOut).ConfigureAwait(false);

		/// <summary>
		/// Return the duration of the bot since the start of the Applications.
		/// </summary>
		protected TimeSpan GetUptime() => InteractivityService.GetUptime();
		/// <summary>
		/// Return the author of the Bot author.
		/// </summary>
		protected IUser GetBotAuthor() => InteractivityService.GetBotAuthor();
		/// <summary>
		/// Return the copyright info.
		/// </summary>
		protected string GetCopyrightInfo() => InteractivityService.GetCopyrightInfo();
	}
}
