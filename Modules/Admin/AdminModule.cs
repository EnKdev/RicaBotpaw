using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace RicaBotpaw.Modules.Admin
{
	public class AdminModule : ModuleBase
	{
		private CommandService _service;
		public AdminModule(CommandService service)
		{
			_service = service;
		}

		[Command("purge")]
		[Remarks("Clears the chat by a specified amount of messages.")]
		public async Task purge([Remainder] int del = 0)
		{
			IGuildUser Bot = await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id);
			if (!Bot.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
			{
				await Context.Channel.SendMessageAsync("`Bot does not have enough permissions to manage messages`");
				return;
			}

			await Context.Message.DeleteAsync();

			var GuildUser = await Context.Guild.GetUserAsync(Context.User.Id);
			if (!GuildUser.GetPermissions(Context.Channel as ITextChannel).ManageMessages)
			{
				await Context.Channel.SendMessageAsync("`You do not have sufficient permissions to manage messages`");
				return;
			}

			if (del == null)
			{
				await Context.Channel.SendMessageAsync("`You need to specify the amount | ;clear (amount) | Replace (amount) with anything`");
			}

			int a = 0;
			foreach (var Item in await Context.Channel.GetMessagesAsync(del).Flatten())
			{
				a++;
				await Item.DeleteAsync();
			}

			await Context.Channel.SendMessageAsync($"`{Context.User.Username} purged {a} messages`");
		}

		[Command("ban")]
		[Remarks("Bans @user")]
		[RequireUserPermission(GuildPermission.BanMembers)]
		[RequireBotPermission(GuildPermission.BanMembers)]
		public async Task BanAsync(SocketGuildUser user = null, [Remainder] string reason = null)
		{
			if (user == null) throw new ArgumentException("You must mention a user");
			if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("You must provide a reason");

			var gld = Context.Guild as SocketGuild;
			var embed = new EmbedBuilder();
			embed.WithColor(new Color(0x4900ff));

			if (user.Id == 112559794543468544)
			{
				await ReplyAsync("Why are you trying to ban my master?! I won't do that...");
			}
			else
			{
				embed.Title = $"**{user.Username}** was banned";
				embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Banned by: **{Context.User.Mention}!\n**Reason: **{reason}";

				await gld.AddBanAsync(user);
				await Context.Channel.SendMessageAsync("", false, embed);
			}
		}

		[Command("kick")]
		[Remarks("Kicks @user")]
		[RequireUserPermission(GuildPermission.KickMembers)]
		[RequireBotPermission(GuildPermission.KickMembers)]
		public async Task KickAsync(SocketGuildUser user, [Remainder] string reason)
		{
			if (user == null) throw new ArgumentException("You must mention a user");
			if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("You must provide a reason");

			if (user.Id == 112559794543468544)
			{
				await ReplyAsync("Why are you trying to get rid of my master?! I won't kick him...");
			}
			else
			{
				var gld = Context.Guild as SocketGuild;
				var embed = new EmbedBuilder();
				embed.WithColor(new Color(0x4900ff));
				embed.Title = $" {user.Username} has been kicked from {user.Guild.Name}";
				embed.Description = $"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Kicked by: **{Context.User.Mention}!\n**Reason: **{reason}";

				await user.KickAsync();
				await Context.Channel.SendMessageAsync("", false, embed);
			}
		}
	}
}