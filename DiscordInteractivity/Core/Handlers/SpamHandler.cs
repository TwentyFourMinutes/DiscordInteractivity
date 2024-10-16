﻿using System.Collections.Concurrent;
using Discord.WebSocket;
using DiscordInteractivity.Core.Interactivity;

namespace DiscordInteractivity.Core.Handlers;

internal class SpamHandler : IDisposable
{
    private readonly InteractivityService _service;

    internal readonly ConcurrentDictionary<ulong, SpamData> SpamInformation;

    internal event Func<SocketGuildUser, List<SocketUserMessage>, Task>? SpamDetected;

    /// <summary>
    /// Determines whether this instance is already Disposed or not.
    /// </summary>
    public bool IsDisposed { get; private set; }

    internal SpamHandler(InteractivityService service)
    {
        _service = service;
        SpamInformation = new ConcurrentDictionary<ulong, SpamData>();

        _service.DiscordClient.MessageReceived += MessageReceived;
    }

    private Task MessageReceived(SocketMessage arg)
    {
        if (
            arg.Author.Id == _service.DiscordClient.CurrentUser.Id
            || arg is not SocketUserMessage message
            || (
                _service.Config.IgnoreRolesPosition != -1
                && ((SocketGuildUser)message.Author).Roles.Max(x => x.Position)
                    < _service.Config.IgnoreRolesPosition
            )
        )
        {
            return Task.CompletedTask;
        }

        if (SpamInformation.TryGetValue(arg.Author.Id, out var info))
        {
            if (info.Messages.Count >= _service.Config.SpamCount)
            {
                if (info.SpamReset <= DateTime.UtcNow)
                {
                    info.SpamReset = DateTime.UtcNow.Add(_service.Config.SpamDuration);
                }
                else
                {
                    _ = SpamDetected?.Invoke((SocketGuildUser)arg.Author, info.Messages);
                }

                info.Messages.Clear();
            }

            info.Messages.Add(message);
        }
        else
        {
            SpamInformation.TryAdd(
                arg.Author.Id,
                new SpamData
                {
                    SpamReset = DateTime.UtcNow.Add(_service.Config.SpamDuration),
                    Messages = new List<SocketUserMessage>(),
                }
            );
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            _service.DiscordClient.MessageReceived -= MessageReceived;
            IsDisposed = true;
        }
    }
}

internal class SpamData
{
    internal required DateTime SpamReset { get; set; }
    internal required List<SocketUserMessage> Messages { get; init; }
}
