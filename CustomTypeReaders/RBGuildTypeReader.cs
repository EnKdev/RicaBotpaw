// RBGuildTypeReader.cs
// Original File made by AntiTcb

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RicaBotpaw.TypeReaders
{
	public class RBGuildTypeReader<T> : TypeReader where T : class, IGuild
	{
		private readonly DiscordSocketClient _client;

		public override async Task<TypeReaderResult> Read(ICommandContext ctx, string input, IServiceProvider services)
		{
			var results = new Dictionary<ulong, TypeReaderValue>();
			var guilds = await ctx.Client.GetGuildsAsync(CacheMode.CacheOnly).ConfigureAwait(false);

			// (By id, V 1.0 - 1.0.2)
			if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out ulong id))
				AddResult(results, await ctx.Client.GetGuildAsync(id, CacheMode.CacheOnly).ConfigureAwait(false) as T, 1.00f);

			// By name ( V 0.8 - 0.9)
			foreach (var guild in guilds.Where(g => string.Equals(input, g.Name, StringComparison.OrdinalIgnoreCase)))
				AddResult(results, guild as T, guild.Name == input ? 0.9f : 0.8f);

			if (results.Count > 0)
				return TypeReaderResult.FromSuccess(results.Values);

			return TypeReaderResult.FromError(CommandError.ObjectNotFound, "Guild not found");
		}

		private void AddResult(Dictionary<ulong, TypeReaderValue> results, T guild, float score)
		{
			if (guild != null && !results.ContainsKey(guild.Id))
				results.Add(guild.Id, new TypeReaderValue(guild, score));
		}
	}
}
