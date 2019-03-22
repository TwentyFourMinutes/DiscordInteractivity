using Discord;
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
	public static class InteractivityExtensions
	{
		private static InteractivityService _InteractivityInstance;

		public static async Task<bool> TryDeleteAsync(this IDeletable deletable)
		{
			try
			{
				await deletable.DeleteAsync().ConfigureAwait(false);
			}
			catch { return false; }
			return true;
		}
		
		public static async Task<IUserMessage> SendAndDeleteMessageAsync(this IMessageChannel channel, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			if (_InteractivityInstance is null)
				throw new InvalidOperationException("The InterativityInstance has to be set!");
			IUserMessage msg = await channel.SendMessageAsync(text, isTTS, embed, options).ConfigureAwait(false);
			DeleteMessageAfter(msg, timeOut);
			return msg;
		}
		public static async Task<IUserMessage> SendAndDeleteFileAsync(this IMessageChannel channel, string filePath, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			if (_InteractivityInstance is null)
				throw new InvalidOperationException("The InterativityInstance has to be set!");
			IUserMessage msg = await channel.SendFileAsync(filePath, text, isTTS, embed, options).ConfigureAwait(false);
			DeleteMessageAfter(msg, timeOut);
			return msg;
		}
		public static async Task<IUserMessage> SendAndDeleteFileAsync(this IMessageChannel channel, Stream stream, string filename, string text = null, bool isTTS = false, Embed embed = null, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			if (_InteractivityInstance is null)
				throw new InvalidOperationException("The InterativityInstance has to be set!");
			IUserMessage msg = await channel.SendFileAsync(stream, filename, text, isTTS, embed, options).ConfigureAwait(false);
			DeleteMessageAfter(msg, timeOut);
			return msg;
		}
		public static async Task<IUserMessage> SendPaginatorAsync(this IMessageChannel channel, Paginator paginator, TimeSpan? timeOut = null, RequestOptions options = null)
		{
			if (_InteractivityInstance is null)
				throw new InvalidOperationException("The InterativityInstance has to be set!");
			return await paginator.Initialize(_InteractivityInstance, channel, timeOut).ConfigureAwait(false);
		}

		public static async Task<WaitingMessageResult> WaitForMessageAsync(this IMessageChannel channel, IUser user, bool ignoreCommands = true, TimeSpan? timeOut = null)
		{
			if (_InteractivityInstance is null)
				throw new InvalidOperationException("The InterativityInstance has to be set!");

			var tcs = new TaskCompletionSource<SocketMessage>();

			Task MessageReceived(SocketMessage arg)
			{
				if (arg.Channel.Id != channel.Id || arg.Author.Id != user.Id || (ignoreCommands && _InteractivityInstance.Config.CommandPrefixes.Any(x => arg.Content.StartsWith(x))))
					return Task.CompletedTask;

				tcs.SetResult(arg);

				return Task.CompletedTask;
			}

			_InteractivityInstance.DiscordClient.MessageReceived += MessageReceived;

			var trigger = tcs.Task;
			var task = await Task.WhenAny(trigger, Task.Delay(timeOut ?? _InteractivityInstance.Config.DefaultWaitingTimeout)).ConfigureAwait(false);

			_InteractivityInstance.DiscordClient.MessageReceived -= MessageReceived;

			if (task == trigger)
			{
				return new WaitingMessageResult { Message = await trigger.ConfigureAwait(false), Result = Result.UserResponded };
			}
			else
			{
				return new WaitingMessageResult { Result = Result.TimedOut };
			}
		}
		public static async Task<WaitingReactionResult> WaitForReactionAsync(this IMessageChannel channel, IUser user, TimeSpan? timeOut = null)
		{
			if (_InteractivityInstance is null)
				throw new InvalidOperationException("The InterativityInstance has to be set!");

			var tcs = new TaskCompletionSource<SocketReaction>();

			Task ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
			{
				if (arg2.Id != channel.Id || arg3.UserId != user.Id)
					return Task.CompletedTask;

				tcs.SetResult(arg3);

				return Task.CompletedTask;
			}

			_InteractivityInstance.DiscordClient.ReactionAdded += ReactionAdded;

			var trigger = tcs.Task;
			var task = await Task.WhenAny(trigger, Task.Delay(timeOut ?? _InteractivityInstance.Config.DefaultWaitingTimeout)).ConfigureAwait(false);

			_InteractivityInstance.DiscordClient.ReactionAdded -= ReactionAdded;

			if (task == trigger)
			{
				return new WaitingReactionResult { Message = await trigger.ConfigureAwait(false), Result = Result.UserResponded };
			}
			else
			{
				return new WaitingReactionResult { Result = Result.TimedOut };
			}
		}

		public static void SetInteractivityInstance(InteractivityService service)
		{
			_InteractivityInstance = service;
		}

		#region HelperMethods
		private static void DeleteMessageAfter(IUserMessage msg, TimeSpan? timeOut)
		{
			_ = Task.Delay(timeOut ?? _InteractivityInstance.Config.DefaultMessageTimeout).ContinueWith(_ => msg.DeleteAsync().ConfigureAwait(false)).ConfigureAwait(false);
		}
		#endregion
	}
}
