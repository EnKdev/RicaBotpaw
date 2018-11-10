// RBRatelimitAttribute.cs
// Adds an attribute to ratelimit the invocation of commands per user.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace RicaBotpaw.Attributes
{
	// Modification of Discord.Addons.Preconditions.RatelimitAttribute
	// https://github.com/Joe4evr/Discord.Addons/blob/master/src/Discord.Addons.Preconditions/Ratelimit/RatelimitAttribute.cs
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class RBRatelimitAttribute : PreconditionAttribute
	{
		private readonly uint invokeLimit;
		private readonly bool noLimitInDMs;
		private readonly bool noLimitForAdmins;
		private readonly bool applyPerGuild;
		private readonly TimeSpan invokeLimitPeriod;

		private readonly Dictionary<(ulong, ulong?), CommandTimeout> invokeTracker =
			new Dictionary<(ulong, ulong?), CommandTimeout>();

		public RBRatelimitAttribute(
			uint times,
			double period,
			Measure measure,
			RatelimitFlags flags = RatelimitFlags.None)
		{
			invokeLimit = times;
			noLimitInDMs = (flags & RatelimitFlags.NoLimitInDMs) == RatelimitFlags.NoLimitInDMs;
			noLimitForAdmins = (flags & RatelimitFlags.NoLimitForAdmins) == RatelimitFlags.NoLimitForAdmins;
			applyPerGuild = (flags & RatelimitFlags.ApplyPerGuild) == RatelimitFlags.ApplyPerGuild;

			switch (measure)
			{
				case Measure.Days:
					invokeLimitPeriod = TimeSpan.FromDays(period);
					break;
				case Measure.Hours:
					invokeLimitPeriod = TimeSpan.FromHours(period);
					break;
				case Measure.Minutes:
					invokeLimitPeriod = TimeSpan.FromMinutes(period);
					break;
				case Measure.Seconds:
					invokeLimitPeriod = TimeSpan.FromSeconds(period);
					break;
			}
		}

		public RBRatelimitAttribute(
			uint times,
			TimeSpan period,
			RatelimitFlags flags = RatelimitFlags.None)
		{
			invokeLimit = times;
			noLimitInDMs = (flags & RatelimitFlags.NoLimitInDMs) == RatelimitFlags.NoLimitInDMs;
			noLimitForAdmins = (flags & RatelimitFlags.NoLimitForAdmins) == RatelimitFlags.NoLimitForAdmins;
			applyPerGuild = (flags & RatelimitFlags.ApplyPerGuild) == RatelimitFlags.ApplyPerGuild;

			invokeLimitPeriod = period;
		}

		public override Task<PreconditionResult> CheckPermissions(
			ICommandContext context,
			CommandInfo command,
			IServiceProvider services)
		{
			if (noLimitInDMs && context.Channel is IPrivateChannel)
				return Task.FromResult(PreconditionResult.FromSuccess());

			if (noLimitForAdmins && context.User is IGuildUser gu && gu.GuildPermissions.Administrator)
				return Task.FromResult(PreconditionResult.FromSuccess());

			var now = DateTime.UtcNow;
			var key = applyPerGuild ? (context.User.Id, context.Guild?.Id) : (context.User.Id, null);

			var timeout = (invokeTracker.TryGetValue(key, out var t)
			               && ((now - t.FirstInvoke) < invokeLimitPeriod))
				? t
				: new CommandTimeout(now);

			timeout.TimesInvoked++;

			if (timeout.TimesInvoked <= invokeLimit)
			{
				invokeTracker[key] = timeout;
				return Task.FromResult(PreconditionResult.FromSuccess());
			}
			else
			{
				return Task.FromResult(PreconditionResult.FromError("You are currently in Timeout"));
			}
		}

		private sealed class CommandTimeout
		{
			public uint TimesInvoked { get; set; }
			public DateTime FirstInvoke { get; }

			public CommandTimeout(DateTime timeStarted)
			{
				FirstInvoke = timeStarted;
			}
		}
	}


	public enum Measure
	{
		Days,
		Hours,
		Minutes,
		Seconds
	}

	[Flags]
	public enum RatelimitFlags
	{
		None = 0,
		NoLimitInDMs = 1 << 0,
		NoLimitForAdmins = 1 << 1,
		ApplyPerGuild = 1 << 2
	}
}
