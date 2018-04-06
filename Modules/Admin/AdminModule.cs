using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace RicaBotpaw.Modules.Admin
{
	/// <summary>
	///     This is the class for all of the bots admintools
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	[Remarks("This is the administrative module for server owners.")]
	public class Admintools : ModuleBase // Exempt from configs
	{
		/// <summary>
		///     The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		///     This registers the AdminModule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Admintools(CommandService service)
		{
			_service = service;
		}

		/// <summary>
		/// Purges the chat by x messages
		/// </summary>
		/// <param name="del">The delete.</param>
		/// <returns></returns>
		[Command("purge", RunMode = RunMode.Async)]
		[Remarks("Clears the chat by a specified amount of messages.")]
		[RequireUserPermission(GuildPermission.ManageMessages)]
		public async Task purge([Remainder] int msgToDelete = 0)
		{
			var Bot = await Context.Guild.GetUserAsync(Context.Client.CurrentUser.Id);
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

			if (msgToDelete == null)
				await Context.Channel.SendMessageAsync(
					"`You need to specify the amount | ;clear (amount) | Replace (amount) with anything`");

			var a = 0;
			foreach (var Item in await Context.Channel.GetMessagesAsync(msgToDelete).Flatten())
			{
				a++;
				await Item.DeleteAsync();
			}

			await Context.Channel.SendMessageAsync($"`{Context.User.Username} purged {a} messages`");
		}


		/// <summary>
		/// Bans a bad user
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="reason">The reason.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">
		/// You must mention a user
		/// or
		/// You must provide a reason
		/// </exception>
		[Command("ban", RunMode = RunMode.Async)]
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
				embed.Description =
					$"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Banned by: **{Context.User.Mention}!\n**Reason: **{reason}";

				await gld.AddBanAsync(user);
				await Context.Channel.SendMessageAsync("", false, embed);
			}
		}


		/// <summary>
		/// Kicks a bad user
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="reason">The reason.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">You must mention a user
		/// or
		/// You must provide a reason</exception>
		[Command("kick", RunMode = RunMode.Async)]
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
				embed.Description =
					$"**Username: **{user.Username}\n**Guild Name: **{user.Guild.Name}\n**Kicked by: **{Context.User.Mention}!\n**Reason: **{reason}";

				await user.KickAsync();
				await Context.Channel.SendMessageAsync("", false, embed);
			}
		}
	}
}