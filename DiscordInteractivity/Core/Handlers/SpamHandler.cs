using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core.Handlers
{
	internal class SpamHandler : IDisposable
	{
		private readonly InteractivityService _service;

		internal readonly ConcurrentDictionary<ulong, SpamData> SpamInformation;

		internal event Func<SocketUser, Task> SpamDetected;

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
			if (arg.Author.Id == _service.DiscordClient.CurrentUser.Id)
				return Task.CompletedTask;

			if (SpamInformation.TryGetValue(arg.Author.Id, out var info))
			{
				if (info.MessageCount >= _service.Config.SpamCount)
				{
					if (info.SpamReset <= DateTime.UtcNow)
					{
						info.MessageCount = 0;
						info.SpamReset = DateTime.UtcNow.Add(_service.Config.SpamDuration);
					}
					else
					{
						_ = SpamDetected?.Invoke(arg.Author);
					}
				}

				info.MessageCount++;
			}
			else
			{
				SpamInformation.TryAdd(arg.Author.Id, new SpamData { SpamReset = DateTime.UtcNow.Add(_service.Config.SpamDuration), MessageCount = 1 });
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
		internal DateTime SpamReset { get; set; }
		internal int MessageCount { get; set; }
	}
}
