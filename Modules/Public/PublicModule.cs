using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules.Public
{

	public class PublicModule : ModuleBase
	{
		private CommandService _service;

		public PublicModule(CommandService service)
		{
			_service = service;
		}

		[Command("help")]
		[Remarks("Shows a list of all available commands per module")]
		public async Task HelpAsync()
		{
			var dmChannel = await Context.User.CreateDMChannelAsync();

			string prefix = ";";
			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = "These are the commands you can use"
			};

			foreach (var module in _service.Modules)
			{
				string description = null;
				foreach (var cmd in module.Commands)
				{
					var result = await cmd.CheckPreconditionsAsync(Context);
					if (result.IsSuccess)
						description += $"{prefix}{cmd.Aliases.First()}\n";
				}

				if (!string.IsNullOrWhiteSpace(description))
				{
					builder.AddField(x =>
					{
						x.Name = module.Name;
						x.Value = description;
						x.IsInline = false;
					});
				}
			}

			await dmChannel.SendMessageAsync("", false, builder.Build());
		}

		[Command("help")]
		[Remarks("Shows what a specific command does and what parameters it takes.")]
		public async Task HelpAsync(string command)
		{
			var dmChannel = await Context.User.CreateDMChannelAsync();
			var result = _service.Search(Context, command);

			if (!result.IsSuccess)
			{
				await ReplyAsync($"Sorry, but it seems that i don't know a command like **{command}**...");
				return;
			}

			string prefix = ";";
			var builder = new EmbedBuilder()
			{
				Color = new Color(114, 137, 218),
				Description = $"Here are some commands like **{command}**"
			};

			foreach (var match in result.Commands)
			{
				var cmd = match.Command;

				builder.AddField(x =>
				{
					x.Name = string.Join(", ", cmd.Aliases);
					x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" + $"Remarks: {cmd.Remarks}";
					x.IsInline = false;
				});
			}

			await dmChannel.SendMessageAsync("", false, builder.Build());
		}

		[Command("setgame")]
		[Remarks("Sets a new game for the bot")]
		public async Task setGame([Remainder] string game)
		{
			if (!(Context.User.Id == 112559794543468544))
			{
				await Context.Channel.SendMessageAsync("You do not have permission to change my game. Contact EnK_#8906 if you think this is wrong");
			}
			else
			{
				await (Context.Client as DiscordSocketClient).SetGameAsync(game);
				await Context.Channel.SendMessageAsync($"Successfully set the game to *{game}*");
				Console.WriteLine($"{DateTime.Now}: Game was changed to {game}");
			}
		}

		[Command("botinfo")]
		[Remarks("Shows all of the bot info")]
		public async Task Info()
		{
			using (var process = Process.GetCurrentProcess())
			{
				var embed = new EmbedBuilder();
				var application = await Context.Client.GetApplicationInfoAsync();
				embed.ImageUrl = application.IconUrl;
				embed.WithColor(new Color(0x4900ff))
				.AddField(y =>
				{
					y.Name = "Author";
					y.Value = application.Owner.Username; application.Owner.Id.ToString();
					y.IsInline = false;
				})
				.AddField(y =>
				{
					y.Name = "Uptime";
					var time = DateTime.Now - process.StartTime;
					var sb = new StringBuilder();

					if (time.Days > 0)
					{
						sb.Append($"{time.Days}d ");
					}

					if (time.Hours > 0)
					{
						sb.Append($"{time.Hours}h ");
					}

					if (time.Minutes > 0)
					{
						sb.Append($"{time.Minutes}m ");
					}

					sb.Append($"{time.Seconds}s ");
					y.Value = sb.ToString();
					y.IsInline = true;
				})
				.AddField(y =>
				{
					y.Name = "Discord.NET Version";
					y.Value = DiscordConfig.Version;
					y.IsInline = true;
				})
				.AddField(y =>
				{
					y.Name = "Heap size";
					y.Value = GetHeapSize();
					y.IsInline = false;
				})
				.AddField(y =>
				{
					y.Name = "Members";
					y.Value = (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Users.Count).ToString();
					y.IsInline = false;
				})
				.AddField(y =>
				{
					y.Name = "Channels";
					y.Value = (Context.Client as DiscordSocketClient).Guilds.Sum(g => g.Channels.Count).ToString();
					y.IsInline = false;
				});

				await this.ReplyAsync("", embed: embed);
			}
		}

		private static string GetUptime() => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
		private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();

		[Command("userinfo")]
		[Alias("uinfo")]
		[Name("userinfo `<user>`")]
		[Remarks("Returns one users info")]
		public async Task UserInfo(IGuildUser user)
		{
			var application = await Context.Client.GetApplicationInfoAsync();
			var thumbnailUrl = user.GetAvatarUrl(ImageFormat.Png);
			var date = $"{user.CreatedAt.Day}/{user.CreatedAt.Month}/{user.CreatedAt.Year}";

			var auth = new EmbedAuthorBuilder()
			{
				Name = user.Username,
				IconUrl = thumbnailUrl
			};

			var embed = new EmbedBuilder()
			{
				Color = new Color(29, 140, 209),
				Author = auth
			};

			var us = user as SocketGuildUser;

			var D = us.Username;

			var A = us.Discriminator;
			var T = us.Id;
			var S = date;
			var C = us.Status;
			var CC = us.JoinedAt;
			var O = us.Game;
			embed.Title = $"**{us.Username}** Information";
			embed.Description = $"Username: **{D}**\nDiscriminator: **{A}**\nUser ID: **{T}**\nCreated at: **{S}**\nCurrent Status: **{C}**\nJoined server at: **{CC}**\nPlaying: **{O}**";

			await ReplyAsync("", false, embed.Build());
		}

		[Command("serverinfo")]
		[Alias("sinfo", "serv")]
		[Remarks("Info about a server this bot is in")]
		public async Task GuildInfo()
		{
			EmbedBuilder embedBuilder;
			embedBuilder = new EmbedBuilder();
			embedBuilder.WithColor(new Color(0, 71, 171));

			var gld = Context.Guild as SocketGuild;
			var cli = Context.Client as DiscordSocketClient;

			if (!string.IsNullOrWhiteSpace(gld.IconUrl))
				embedBuilder.ThumbnailUrl = gld.IconUrl;

			var O = gld.Owner.Username;
			var V = gld.VoiceRegionId;
			var C = gld.CreatedAt;
			var N = gld.DefaultMessageNotifications;
			var R = gld.Roles;
			var VL = gld.VerificationLevel;
			var XD = gld.Roles.Count;
			var X = gld.MemberCount;
			var Z = cli.ConnectionState;

			embedBuilder.Title = $"{gld.Name} Server Information";
			embedBuilder.Description = $"Server Owner: **{O}\n**Voice Region: **{V}\n**Created at: **{C}\n**MsgNtfc: **{N}\n**Verification: **{VL}\n**Role Count: **{XD}\n**Members: **{X}\n**Connection state: **{Z}\n\n**";
			await ReplyAsync("", false, embedBuilder);
		}

		private static IUser me;

		[Command("ownerDM")]
		[Remarks("Sends a DM to the owner. Useful for bug reports")]
		public async Task dm([Remainder] string dm)
		{
			var myId = Context.User.Mention;
			if (me == null)
			{
				foreach (var user in Context.Guild.GetUsersAsync().Result)
				{
					if (user.Id == 112559794543468544) // Don't change this. This is for forwarding messages to me if you want to request something for the bot!
					{
						me = user;
						myId = user.Mention;
						break;
					}
				}
			}

			var application = await Context.Client.GetApplicationInfoAsync();
			var message = await application.Owner.CreateDMChannelAsync();
			var embed = new EmbedBuilder()
			{
				Color = new Color(0, 0, 255)
			};

			embed.Description = $"{dm}";
			embed.WithFooter(new EmbedFooterBuilder().WithText($"Message from: {Context.User.Username} | Guild: {Context.Guild.Name}"));

			await message.SendMessageAsync("", false, embed);
			embed.Description = $"You have sent a message to {me}. He will read the message soon.";
			await Context.Channel.SendMessageAsync("", false, embed);
		}

		[Command("changelog")]
		[Remarks("Returns Ricas Changelog which includes her version")]
		public async Task Changelog()
		{
			await ReplyAsync(System.IO.File.ReadAllText(@"PATH TO YOUR CHANGELOG"));
		}
		

		// Database stuff

		[Command("status")]
		[Alias("s")]
		[Remarks("Retrieves data about a user from the Database")]
		public async Task dbSay([Remainder] IUser user = null)
		{
			var embed = new EmbedBuilder()
			{
				Color = new Color(0, 0, 255)
			};

			if (user == null)
			{
				user = Context.User;
			}

			var result = Database.CheckExistingUser(user);

			if (result.Count() <= 0)
			{
				Database.EnterUser(user);
			}

			var tableName = Database.GetUserStatus(user);
			embed.Description = (Context.User.Mention + "\n\n" + user.Username + "'s current status is as followed: \n" 
				+ ":small_blue_diamond:" + "UserID: " + tableName.FirstOrDefault().UserId + "\n" 
				+ ":small_blue_diamond:" + tableName.FirstOrDefault().Tokens + " tokens!\n"
				+ ":small_blue_diamond: Current custom rank: " + tableName.FirstOrDefault().Rank + "\n"
				+ ":dollar: " + tableName.FirstOrDefault().Money + " Rica-Dollar!\n"
				+ ":small_blue_diamond: Level: " + tableName.FirstOrDefault().Level + "\n"
				+ ":small_blue_diamond: XP: " + tableName.FirstOrDefault().XP);

			await Context.Channel.SendMessageAsync("", false, embed);
		}

		[Command("enterDb")]
		[Remarks("Enters you into Ricas Database")]
		public async Task dbEnter([Remainder] IUser user = null)
		{
			if (user == null)
			{
				user = Context.User;
			}

			var enterDb = Database.EnterUser(user);
			await ReplyAsync("You should be entered now. If not, then hell...");
		}

		// Database Awards, UwU
		[Command("award")]
		[Remarks("Award someone with some tokens, Woo!")]
		public async Task Award(SocketGuildUser user, [Remainder] uint tokens)
		{
			if (tokens > 50)
			{
				await ReplyAsync(Context.User.Mention + ", Woah there. The amount you entered was too high to handle.\nKeep it at least below or equal to 50!");
			}
			else
			{
				Database.ChangeTokens(user, tokens);
				await ReplyAsync(user.Mention + ", you were awarded with " + tokens + " tokens!");
			}
		}
    }
}