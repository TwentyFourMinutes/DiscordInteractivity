using System.Collections.Concurrent;
using Discord.Commands;
using DiscordInteractivity.Enums;

namespace DiscordInteractivity.Attributes;

/// <summary>
/// Sets a ratelimit for a command or a module for each user.
/// </summary>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = false,
    Inherited = false
)]
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

    internal ConcurrentDictionary<ulong, TimeoutData> Cooldowns;

    internal static readonly List<CooldownAttribute> CooldownAttributes = [];

    /// <summary>
    /// This event gets fired as soon as a command is not executed because a user is on cooldown.
    /// </summary>
    public static event Func<CooldownAttribute, TimeSpan, Task>? UserOnCooldown;

    /// <summary>
    /// Attaches a cooldown per user to your command.
    /// </summary>
    /// <param name="count">The number of times in which the user is allowed to invoke this command in the given timespan.</param>
    /// <param name="every">The timespan in which the user is allowed to invoke this command <see cref="Count"/> times.</param>
    /// <param name="measure">The <see cref="CooldownMeasure"/> to convert <paramref name="every"/> with.</param>
    public CooldownAttribute(int count, int every, CooldownMeasure measure)
    {
        Count = count;

        Cooldowns = new ConcurrentDictionary<ulong, TimeoutData>();

        Every = measure switch
        {
            CooldownMeasure.Seconds => TimeSpan.FromSeconds(every),
            CooldownMeasure.Minutes => TimeSpan.FromMinutes(every),
            CooldownMeasure.Hours => TimeSpan.FromHours(every),
            _ => throw new ArgumentOutOfRangeException(nameof(measure), measure, null),
        };

        CooldownAttributes.Add(this);
    }

    public override Task<PreconditionResult> CheckPermissionsAsync(
        ICommandContext context,
        CommandInfo command,
        IServiceProvider services
    )
    {
        if (Cooldowns.TryGetValue(context.User.Id, out var data))
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
            Cooldowns.TryAdd(
                context.User.Id,
                new TimeoutData { NextReset = DateTime.UtcNow.Add(Every), InvokeCount = 1 }
            );
        }

        return Task.FromResult(PreconditionResult.FromSuccess());
    }
}

internal class TimeoutData
{
    internal DateTime NextReset;
    internal long InvokeCount;
}
