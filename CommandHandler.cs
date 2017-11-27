using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using RicaBotpaw.Modules.Data;
using Discord;
using System;
using System.Diagnostics;

namespace RicaBotpaw
{
	/// <summary>
	/// This is the commandhandler where all modules get registered
	/// </summary>
	public class CommandHandler
    {
		/// <summary>
		/// The CMDS
		/// </summary>
		private CommandService _cmds;
		/// <summary>
		/// The client
		/// </summary>
		private DiscordSocketClient _client;

		/// <summary>
		/// This "installs" all modules into the handler and registers their commands
		/// </summary>
		/// <param name="c">The c.</param>
		/// <returns></returns>
		public async Task Install(DiscordSocketClient c)
        {
            _client = c;
            _cmds = new CommandService();

            await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());

            _client.MessageReceived += HandleCommand;
			_client.Ready += onReady;
			_client.UserJoined += onJoin;
			_client.UserLeft += onLeave;
			_client.MessageReceived += giveXP;
        }

		/// <summary>
		/// This is the actual commandhandler
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns></returns>
		public async Task HandleCommand(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;

            var context = new CommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(";", ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _cmds.ExecuteAsync(context, argPos);

                if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ToString());
            }
        }

		/// <summary>
		/// When the bot is ready, this will be triggered
		/// </summary>
		/// <returns></returns>
		public async Task onReady()
		{
			await _client.SetGameAsync("For help, use ;help");
		}

		/// <summary>
		/// When a new user joins a guild, this will be triggered
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public async Task onJoin(SocketGuildUser user)
		{
			var channel = user.Guild.DefaultChannel;
			await channel.SendMessageAsync(user.Username + " has entered the server! Say hi!");
		}

		/// <summary>
		/// When a user leaves a guild, this will be triggered
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public async Task onLeave(SocketGuildUser user)
		{
			var channel = user.Guild.DefaultChannel;
			await channel.SendMessageAsync(user.Username + " has left us alone! Parting is such a sweet sorrow...");
		}

		/// <summary>
		/// This is what gives a user the XP and also it levels them up.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		public async Task giveXP(SocketMessage msg)
		{
			var user = msg.Author;
			var result = Database.CheckExistingUser(user);

			if (result.Count <= 0 && user.IsBot != true)
			{
				Database.EnterUser(user);
			}

			var userData = Database.GetUserStatus(user).FirstOrDefault();
			var xp = XP.returnXP(msg);
			var xpToLevelUp = XP.calculateNextLevel(userData.Level);

			if (userData.XP >= xpToLevelUp)
			{
				Database.levelUp(user, xp);
				Database.AddMoney(user);

				var embed = new EmbedBuilder();
				embed.WithColor(new Color(0x4d006d)).AddField(y =>
				{
					var userData2 = Database.GetUserStatus(user).FirstOrDefault();
					y.Name = "Level Up!";
					y.Value = $"{user.Mention} has leveled up to level {userData2.Level}!";
				});

				await msg.Channel.SendMessageAsync("", embed: embed);
			}
			else if (user.IsBot != true)
			{
				Database.addXP(user, xp);
			}
		}
	}
}