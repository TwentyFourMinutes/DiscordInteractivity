using Discord.Commands;
using DiscordInteractivity.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordInteractivity.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class CooldownAttribute : PreconditionAttribute
	{
		/// <summary>
		/// Gets the number of times in which the user is allowed to invoke this command in the given timespan.
		/// </summary>
		public int Count { get; }
		/// <summary>
		/// Gets the timespan in which the user is allowed to invoke this command <see cref="Count"/> times.
		/// </summary>
		public TimeSpan Every { get; }
		/// <summary>
		/// Gets whether the cooldown cache of this command should be auto cleared every 10 minutes or not.
		/// </summary>
		public bool IsToBeCleared = true;

		private ConcurrentDictionary<ulong, TimeoutData> _cooldowns;

		private static readonly List<CooldownAttribute> _cooldownAttributes = new List<CooldownAttribute>();

		private static readonly Timer _timer = new Timer(_ =>
		{
			Task.Run(() =>
			{
				foreach (var attribute in _cooldownAttributes)
				{
					if (attribute.IsToBeCleared)
						foreach (var cooldown in attribute._cooldowns)
						{
							if (cooldown.Value.NextReset <= DateTime.UtcNow)
								attribute._cooldowns.TryRemove(cooldown.Key, out TimeoutData _);
						}
				}
			});
		}, null, 600000, 600000);

		public static event Func<CooldownAttribute, TimeSpan, Task> UserOnCooldown;
		
		/// <summary>
		/// Attaches a cooldown per user to your command.
		/// </summary>
		/// <param name="count">The number of times in which the user is allowed to invoke this command in the given timespan.</param>
		/// <param name="every">The timespan in which the user is allowed to invoke this command <see cref="Count"/> times.</param>
		/// <param name="measure">The <see cref="CooldownMeasure"/> to convert <paramref name="every"/> with.</param>
		public CooldownAttribute(int count, int every, CooldownMeasure measure)
		{
			Count = count;

			_cooldowns = new ConcurrentDictionary<ulong, TimeoutData>();

			switch (measure)
			{
				case CooldownMeasure.Seconds:
					Every = TimeSpan.FromSeconds(every);
					break;
				case CooldownMeasure.Minutes:
					Every = TimeSpan.FromMinutes(every);
					break;
				case CooldownMeasure.Hours:
					Every = TimeSpan.FromHours(every);
					break;
			}

			_cooldownAttributes.Add(this);
		}

		public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
		{
			if (_cooldowns.TryGetValue(context.User.Id, out TimeoutData data))
			{
				if (data.InvokeCount >= Count)
				{
					var now = DateTime.UtcNow;

					if (data.NextReset <= now)
					{
						data.NextReset = now.Add(Every);
						data.InvokeCount = 0;
					}
					else
					{
						_ = UserOnCooldown?.Invoke(this, data.NextReset - now);
						return Task.FromResult(PreconditionResult.FromError("User is on timeout!"));
					}
				}
				data.InvokeCount++;
			}
			else
			{
				_cooldowns.TryAdd(context.User.Id, new TimeoutData { NextReset = DateTime.UtcNow.Add(Every), InvokeCount = 1 });
			}
			return Task.FromResult(PreconditionResult.FromSuccess());
		}
	}
	internal class TimeoutData
	{
		internal DateTime NextReset;
		internal long InvokeCount;
	}
}
