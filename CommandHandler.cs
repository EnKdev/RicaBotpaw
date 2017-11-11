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
    public class CommandHandler
    {
        private CommandService _cmds;
        private DiscordSocketClient _client;

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

		public async Task onReady()
		{
			await _client.SetGameAsync("For help, use ;help");
		}

		public async Task onJoin(SocketGuildUser user)
		{
			var channel = user.Guild.DefaultChannel;
			await channel.SendMessageAsync(user.Username + " has entered the Server! Say hi! *wags tail*");
		}

		public async Task onLeave(SocketGuildUser user)
		{
			var channel = user.Guild.DefaultChannel;
			await channel.SendMessageAsync(user.Username + " has left us alone! Parting is such a sweet sorrow...");
		}

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