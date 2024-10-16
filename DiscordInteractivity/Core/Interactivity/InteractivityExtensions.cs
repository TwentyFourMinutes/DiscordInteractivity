﻿using Discord;
using Discord.WebSocket;
using DiscordInteractivity.Enums;
using DiscordInteractivity.Pager;
using DiscordInteractivity.Results;

namespace DiscordInteractivity.Core.Interactivity;

/// <summary>
/// Provides all sort of useful extensions.
/// </summary>
public static class InteractivityExtensions
{
    private static InteractivityService? _interactivityInstance;

    /// <summary>
    ///	Tries to delete the object and returns if it was successful or not
    /// </summary>
    /// <param name="deletable">The object to be deleted.</param>
    /// <param name="options">The options to be used while sending the request.</param>
    public static async Task<bool> TryDeleteAsync(
        this IDeletable deletable,
        RequestOptions? options = null
    )
    {
        try
        {
            await deletable.DeleteAsync(options).ConfigureAwait(false);
        }
        catch
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Sends a message to an <see cref="IMessageChannel"/> and deletes it after the <see cref="TimeSpan"/> elapsed.
    /// </summary>
    /// <param name="channel">The <see cref="IMessageChannel"/> where the message should be send in.</param>
    /// <param name="text">The message to be sent.</param>
    /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
    /// <param name="embed">The <see cref="Embed"/> to be sent.</param>
    /// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the message.</param>
    /// <param name="options">The options to be used while sending the request.</param>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<IUserMessage> SendAndDeleteMessageAsync(
        this IMessageChannel channel,
        string? text = null,
        bool isTTS = false,
        Embed? embed = null,
        TimeSpan? timeOut = null,
        RequestOptions? options = null
    )
    {
        if (_interactivityInstance is null)
            throw new InvalidOperationException("The InterativityInstance has to be set!");
        IUserMessage msg = await channel
            .SendMessageAsync(text, isTTS, embed, options)
            .ConfigureAwait(false);
        DeleteMessageAfter(msg, timeOut);
        return msg;
    }

    /// <summary>
    /// Sends a file to an <see cref="IMessageChannel"/> with an optional caption and deletes it after the <see cref="TimeSpan"/> elapsed.
    /// </summary>
    /// <param name="filePath">The file path of the file to be sent.</param>
    /// <param name="channel">The <see cref="IMessageChannel"/> where the message should be send in.</param>
    /// <param name="text">The message to be sent.</param>
    /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
    /// <param name="embed">The <see cref="Embed"/> to be sent.</param>
    /// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the message.</param>
    /// <param name="options">The options to be used while sending the request.</param>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<IUserMessage> SendAndDeleteFileAsync(
        this IMessageChannel channel,
        string filePath,
        string? text = null,
        bool isTTS = false,
        Embed? embed = null,
        TimeSpan? timeOut = null,
        RequestOptions? options = null
    )
    {
        if (_interactivityInstance is null)
            throw new InvalidOperationException("The InterativityInstance has to be set!");
        IUserMessage msg = await channel
            .SendFileAsync(filePath, text, isTTS, embed, options)
            .ConfigureAwait(false);
        DeleteMessageAfter(msg, timeOut);
        return msg;
    }

    /// <summary>
    /// Sends a file to an <see cref="IMessageChannel"/> with an optional caption and deletes it after the <see cref="TimeSpan"/> elapsed.
    /// </summary>
    /// <param name="stream">The stream of the file to be sent.</param>
    /// <param name="filename">The name of the attachment.</param>
    /// <param name="channel">The <see cref="IMessageChannel"/> where the message should be send in.</param>
    /// <param name="text">The message to be sent.</param>
    /// <param name="isTTS">Determines whether the message should be read aloud by Discord or not.</param>
    /// <param name="embed">The <see cref="Embed"/> to be sent.</param>
    /// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the message.</param>
    /// <param name="options">The options to be used while sending the request.</param>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<IUserMessage> SendAndDeleteFileAsync(
        this IMessageChannel channel,
        Stream stream,
        string filename,
        string? text = null,
        bool isTTS = false,
        Embed? embed = null,
        TimeSpan? timeOut = null,
        RequestOptions? options = null
    )
    {
        if (_interactivityInstance is null)
            throw new InvalidOperationException("The InterativityInstance has to be set!");
        IUserMessage msg = await channel
            .SendFileAsync(stream, filename, text, isTTS, embed, options)
            .ConfigureAwait(false);
        DeleteMessageAfter(msg, timeOut);
        return msg;
    }

    /// <summary>
    /// Sends a <see cref="Paginator"/> to an <see cref="IMessageChannel"/> and deletes it after the <see cref="TimeSpan"/> elapsed.
    /// </summary>
    /// <param name="channel">The <see cref="IMessageChannel"/> where the message should be send in.</param>
    /// <param name="paginator">The <see cref="Paginator"/> to be sent.</param>
    /// <param name="timeOut">The <see cref="TimeSpan"/> it needs to wait before deleting the <see cref="Paginator"/>.</param>
    /// <param name="options">The options to be used while sending the request.</param>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<IUserMessage> SendPaginatorAsync(
        this IMessageChannel channel,
        Paginator? paginator,
        TimeSpan? timeOut = null,
        RequestOptions? options = null
    )
    {
        if (_interactivityInstance is null)
            throw new InvalidOperationException("The InterativityInstance has to be set!");
        return await paginator.Initialize(_interactivityInstance, channel, timeOut);
    }

    /// <summary>
    /// Waits for an <see cref="IUser"/> to sent a message in a specific channel.
    /// </summary>
    /// <param name="channel">The <see cref="IMessageChannel"/> where the message should be waited for.</param>
    /// <param name="user">The <see cref="IUser"/> to be waited for.</param>
    /// <param name="ignoreCommands">Determines whether messages with Command Prefixes should be ignored.</param>
    /// <param name="timeOut">The <see cref="TimeSpan"/> it waits for the user.</param>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<WaitingMessageResult> WaitForMessageAsync(
        this IMessageChannel channel,
        IUser user,
        bool ignoreCommands = true,
        TimeSpan? timeOut = null
    )
    {
        if (_interactivityInstance is null)
            throw new InvalidOperationException("The InterativityInstance has to be set!");

        var tcs = new TaskCompletionSource<SocketMessage>();

        Task MessageReceived(SocketMessage arg)
        {
            if (
                arg.Channel.Id != channel.Id
                || arg.Author.Id != user.Id
                || ignoreCommands
                    && _interactivityInstance.Config.CommandPrefixes.Any(arg.Content.StartsWith)
            )
                return Task.CompletedTask;

            tcs.SetResult(arg);

            return Task.CompletedTask;
        }

        _interactivityInstance.DiscordClient.MessageReceived += MessageReceived;

        var trigger = tcs.Task;
        var task = await Task.WhenAny(
                trigger,
                Task.Delay(timeOut ?? _interactivityInstance.Config.DefaultWaitingTimeout)
            )
            .ConfigureAwait(false);

        _interactivityInstance.DiscordClient.MessageReceived -= MessageReceived;

        if (task == trigger)
        {
            return new WaitingMessageResult
            {
                Message = await trigger.ConfigureAwait(false),
                Result = Result.UserResponded,
            };
        }
        else
        {
            return new WaitingMessageResult { Message = null, Result = Result.TimedOut };
        }
    }

    /// <summary>
    /// Waits for an <see cref="IUser"/> to react in a specific channel.
    /// </summary>
    /// <param name="channel">The <see cref="IMessageChannel"/> where the reaction should be waited for.</param>
    /// <param name="user">The <see cref="IUser"/> to be waited for.</param>
    /// <param name="timeOut">The <see cref="TimeSpan"/> it waits for the user.</param>
    /// <exception cref="InvalidOperationException"/>
    public static async Task<WaitingReactionResult> WaitForReactionAsync(
        this IMessageChannel channel,
        IUser user,
        TimeSpan? timeOut = null
    )
    {
        if (_interactivityInstance is null)
            throw new InvalidOperationException("The InterativityInstance has to be set!");

        var tcs = new TaskCompletionSource<SocketReaction>();

        async Task ReactionAdded(
            Cacheable<IUserMessage, ulong> arg1,
            Cacheable<IMessageChannel, ulong> arg2,
            SocketReaction arg3
        )
        {
            var argChannel = await arg2.GetOrDownloadAsync();

            if (argChannel.Id != channel.Id || arg3.UserId != user.Id)
                return;

            tcs.SetResult(arg3);
        }

        _interactivityInstance.DiscordClient.ReactionAdded += ReactionAdded;

        var trigger = tcs.Task;
        var task = await Task.WhenAny(
                trigger,
                Task.Delay(timeOut ?? _interactivityInstance.Config.DefaultWaitingTimeout)
            )
            .ConfigureAwait(false);

        _interactivityInstance.DiscordClient.ReactionAdded -= ReactionAdded;

        if (task == trigger)
        {
            return new WaitingReactionResult
            {
                Message = await trigger.ConfigureAwait(false),
                Result = Result.UserResponded,
            };
        }
        else
        {
            return new WaitingReactionResult { Message = null, Result = Result.TimedOut };
        }
    }

    /// <summary>
    /// Sets the internal <see cref="InteractivityService"/> instance which is needed for the extension methods.
    /// </summary>
    public static void SetInteractivityInstance(InteractivityService service)
    {
        _interactivityInstance = service;
    }

    #region HelperMethods
    private static void DeleteMessageAfter(IUserMessage msg, TimeSpan? timeOut)
    {
        ArgumentNullException.ThrowIfNull(_interactivityInstance);

        _ = Task.Delay(timeOut ?? _interactivityInstance.Config.DefaultMessageTimeout)
            .ContinueWith(_ => msg.DeleteAsync().ConfigureAwait(false))
            .ConfigureAwait(false);
    }
    #endregion
}
