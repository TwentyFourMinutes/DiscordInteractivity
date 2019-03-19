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
using System.Threading;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core
{
	public class Interactivity : Interactivity<SocketCommandContext> { }

	public class Interactivity<T> : ModuleBase<T> where T : SocketCommandContext
	{
		public InteractivityService InteractivityService { get; set; }

		#region Replys
		protected async Task<RestUserMessage> ReplyAndDeleteAsync(string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			RestUserMessage msg = await Context.Channel.SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
			DeleteMessageAfter(msg, timeOut ?? InteractivityService.Config.DefaultMessageTimeout);
			return msg;
		}
		protected async Task<RestUserMessage> ReplyFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
		{
			return await Context.Channel.SendFileAsync(filePath, text, isTTS, embed, options).ConfigureAwait(false);
		}
		protected async Task<RestUserMessage> ReplyAndDeleteFileAsync(string filePath, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			RestUserMessage msg = await Context.Channel.SendFileAsync(filePath, text, isTTS, embed, options).ConfigureAwait(false);
			DeleteMessageAfter(msg, timeOut ?? InteractivityService.Config.DefaultMessageTimeout);
			return msg;
		}
		protected async Task<RestUserMessage> ReplyFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
		{
			return await Context.Channel.SendFileAsync(stream, filename, text, isTTS, embed, options).ConfigureAwait(false);
		}
		protected async Task<RestUserMessage> ReplyAndDeleteFileAsync(Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			RestUserMessage msg = await Context.Channel.SendFileAsync(stream, filename, text, isTTS, embed, options).ConfigureAwait(false);
			DeleteMessageAfter(msg, timeOut ?? InteractivityService.Config.DefaultMessageTimeout);
			return msg;
		}
		protected async Task<RestUserMessage> ReplyAndDeletePaginatorAsync(Paginator paginator, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			return await paginator.Initialize(InteractivityService, Context, timeOut ?? InteractivityService.Config.DefaultPagerTimeout).ConfigureAwait(false);
		}
		#endregion

		#region Misc
		protected TimeSpan GetUptime() => DateTime.UtcNow - InteractivityService.StartupTime;
		protected IUser GetBotAuthor()
		{
			return InteractivityService.BotOwner;
		}
		protected string GetCopyrightInfo()
		{
			return InteractivityService.CopyrightInfo;
		}
		#endregion

		protected async Task<WaitingResult> WaitForUserMessageAsync(SocketUser user = null, IMessageChannel channel = null, bool IgnoreCommands = true, TimeSpan? timeOut = null)
		{
			if (user is null)
				user = Context.User;
			if (channel is null)
				channel = Context.Channel;
			if (timeOut is null)
				timeOut = InteractivityService.Config.DefaultWaitingTimeout;

			var tcs = new TaskCompletionSource<SocketMessage>();

			Task MessageReceived(SocketMessage arg)
			{
				if (arg.Channel.Id != channel.Id || arg.Author.Id != user.Id || (IgnoreCommands && InteractivityService.Config.CommandPrefixes.Any(x => arg.Content.StartsWith(x))))
					return Task.CompletedTask;

				tcs.SetResult(arg);

				return Task.CompletedTask;
			}

			Context.Client.MessageReceived += MessageReceived;

			var trigger = tcs.Task;
			var task = await Task.WhenAny(trigger, Task.Delay(timeOut.Value)).ConfigureAwait(false);

			Context.Client.MessageReceived -= MessageReceived;

			if (task == trigger)
			{
				return new WaitingResult { Message = await trigger.ConfigureAwait(false), Result = Result.UserResponded };
			}
			else
			{
				return new WaitingResult { Result = Result.TimedOut };
			}
		}

		#region HelperMethods
		private void DeleteMessageAfter(RestUserMessage msg, TimeSpan timeOut)
		{
			_ = Task.Delay(timeOut).ContinueWith(_ => msg.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
		}
		#endregion
	}
}
