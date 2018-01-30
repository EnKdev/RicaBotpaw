using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using RicaBotpaw.Modules.Data;
using RicaBotpaw.ImageCore;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using RicaBotpaw.Config;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Timers;
using ImageSharp.Formats;
using Urban.NET;

namespace RicaBotpaw.Modules.Public
{
	/// <summary>
	/// The public module
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	[Remarks("This is the public module. It contains all available commands for everyone.")]
	public class Public : ModuleBase
	{
		/// <summary>
		/// The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		/// Initializes the publicmodule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Public(CommandService service)
		{
			_service = service;
		}

		/// <summary>
		/// Sometimes you need help...
		/// </summary>
		/// <returns></returns>
		[Command("help", RunMode = RunMode.Async)]
		[Remarks("Shows a list of all available commands per module")]
		public async Task HelpAsync()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				var dmChannel = await Context.User.GetOrCreateDMChannelAsync();

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
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		[Command("mhelp", RunMode = RunMode.Async)]
		[Alias("m")]
		[Remarks("Shows specific information about the modules.")]
		public async Task ModuleHelp()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				var module = _service.Modules;
				var emb = new EmbedBuilder();
				emb.Color = new Color(114, 137, 218);
				emb.Title = ($"Here is the information about all modules");

				foreach (var match in _service.Modules)
				{
					emb.AddField(e =>
					{
						e.Name = ($"**{match.Name}**");
						if (string.IsNullOrWhiteSpace(match.Remarks))
						{
							e.Value = $"*No remarks found*\nNumber of commands in this module: {match.Commands.Count}";
						}
						else
						{
							e.Value = ($"Remarks:\n***{match.Remarks}***\nNumber of commands in the modules: {match.Commands.Count}");
						}
						e.IsInline = false;
					});
				}
				await ReplyAsync("", false, emb.Build());
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}


		/// <summary>
		/// Command help!
		/// </summary>
		/// <param name="command">The command.</param>
		/// <returns></returns>
		[Command("chelp", RunMode = RunMode.Async)]
		[Alias("c")]
		[Remarks("Shows what a specific command does and what parameters it takes.")]
		public async Task HelpAsync(string command)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				var dmChannel = await Context.User.GetOrCreateDMChannelAsync();
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
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Sets the bots game. Only for the owner
		/// </summary>
		/// <param name="game">The game.</param>
		/// <returns></returns>
		[Command("setgame", RunMode = RunMode.Async)]
		[Remarks("Sets a new game for the bot")]
		public async Task SetGame([Remainder] string game)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (!(Context.User.Id == 112559794543468544))
				{
					await Context.Channel.SendMessageAsync(
						"You do not have permission to change my game as only my Master has it.");
					await BotCooldown.Cooldown();
				}
				else
				{
					await (Context.Client as DiscordSocketClient).SetGameAsync(game);
					await Context.Channel.SendMessageAsync($"Successfully set the game to *{game}*");
					Console.WriteLine($"{DateTime.Now}: Game was changed to {game}");
					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		[Command("stop", RunMode = RunMode.Async)]
		[Remarks("Stops the bot. For updating processes.")]
		public async Task StopTask()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (!(Context.User.Id == 112559794543468544))
				{
					await Context.Channel.SendMessageAsync(
						"You are unable to stop the bot. Only EnK_ can stop the bot.");
					await BotCooldown.Cooldown();
				}
				else
				{
					await Context.Channel.SendMessageAsync(
						"Good bye!");
					Environment.Exit(0);
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}


		/// <summary>
		/// Returns the bot info
		/// </summary>
		/// <returns></returns>
		[Command("botinfo", RunMode = RunMode.Async)]
		[Remarks("Shows all of the bot info")]
		public async Task Info()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				using (var process = Process.GetCurrentProcess())
				{
					var embed = new EmbedBuilder();
					var application = await Context.Client.GetApplicationInfoAsync();
					embed.ImageUrl = application.IconUrl;
					embed.WithColor(new Color(0x4900ff))
						// Generic Information
						.AddField(y =>
						{
							y.Name = "Bot Author";
							y.Value = RBConfig.BotAuthor;
							y.IsInline = true;
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
							y.Name = "RBIC Version";
							y.Value = ImageCoreConfig.Version;
							y.IsInline = true;
						})
						.AddField(y =>
						{
							y.Name = "RBIC Build Revision";
							y.Value = ImageCoreConfig.BuildRevision;
							y.IsInline = true;
						})
						.AddField(y =>
						{
							y.Name = "RB Version";
							y.Value = RBConfig.BotVersion;
							y.IsInline = false;
						})
						.AddField(y =>
						{
							y.Name = "RB Build Revision";
							y.Value = RBConfig.BuildRevision;
							y.IsInline = true;
						})
						.AddField(y =>
						{
							y.Name = "RB Modules";
							y.Value = RBConfig.ModuleCount;
							y.IsInline = true;
						})
						.AddField(y =>
						{
							y.Name = "RB Databases";
							y.Value = RBConfig.BotDatabases;
							y.IsInline = true;
						})
						.AddField(y =>
						{
							y.Name = "Github Repository";
							y.Value = "[Github](https://github.com/TheRealDreamzy/RicaBotpaw)";
							y.IsInline = false;
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
					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Calculates the bots heapsize
		/// </summary>
		/// <returns></returns>
		private static string GetHeapSize() =>
			Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();

		/// <summary>
		/// Returns info about a discord user
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("userinfo", RunMode = RunMode.Async)]
		[Alias("uinfo")]
		[Name("userinfo `<user>`")]
		[Remarks("Returns one users info")]
		public async Task UserInfo(IGuildUser user)
		{
			if (BotCooldown.isCooldownRunning == false)
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
				embed.Description =
					$"Username: **{D}**\nDiscriminator: **{A}**\nUser ID: **{T}**\nCreated at: **{S}**\nCurrent Status: **{C}**\nJoined server at: **{CC}**\nPlaying: **{O}**";

				await ReplyAsync("", false, embed.Build());
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Returns info about a discord server
		/// </summary>
		/// <param name="gld">The GLD.</param>
		/// <returns></returns>
		[Command("serverinfo", RunMode = RunMode.Async)]
		[Alias("sinfo", "serv")]
		[Remarks("Info about a server this bot is in")]
		public async Task GuildInfo()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				EmbedBuilder embedBuilder;
				embedBuilder = new EmbedBuilder();
				embedBuilder.WithColor(new Color(0, 71, 171));

				var gld2 = Context.Guild as SocketGuild;
				var cli = Context.Client as DiscordSocketClient;

				if (!string.IsNullOrWhiteSpace(gld2.IconUrl))
					embedBuilder.ThumbnailUrl = gld2.IconUrl;

				var O = gld2.Owner.Username;
				var V = gld2.VoiceRegionId;
				var C = gld2.CreatedAt;
				var N = gld2.DefaultMessageNotifications;
				var R = gld2.Roles;
				var VL = gld2.VerificationLevel;
				var XD = gld2.Roles.Count;
				var X = gld2.MemberCount;
				var Z = cli.ConnectionState;

				embedBuilder.Title = $"{gld2.Name} Server Information";
				embedBuilder.Description =
					$"Server Owner: **{O}\n**" +
					$"Voice Region: **{V}\n**" +
					$"Created at: **{C}\n**" +
					$"MsgNtfc: **{N}\n**" +
					$"Verification: **{VL}\n**" +
					$"Role Count: **{XD}\n**" +
					$"Members: **{X}\n**" +
					$"Connection state: **{Z}\n\n**";
				await ReplyAsync("", false, embedBuilder);
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Me
		/// </summary>
		private static IUser me;

		/// <summary>
		/// Allows the user to send me a dm
		/// </summary>
		/// <param name="dm">The dm.</param>
		/// <returns></returns>
		[Command("ownerDM", RunMode = RunMode.Async)]
		[Remarks("Sends a DM to the owner. Useful for bug reports")]
		public async Task dm([Remainder] string dm)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				var myId = Context.User.Mention;
				if (me == null)
				{
					foreach (var user in Context.Guild.GetUsersAsync().Result)
					{
						if (user.Id == 112559794543468544)
						{
							me = user;
							myId = user.Mention;
							break;
						}
					}
				}

				var application = await Context.Client.GetApplicationInfoAsync();
				var message = await application.Owner.GetOrCreateDMChannelAsync();
				var embed = new EmbedBuilder()
				{
					Color = new Color(0, 0, 255)
				};

				embed.Description = $"{dm}";
				embed.WithFooter(
					new EmbedFooterBuilder().WithText($"Message from: {Context.User.Username} | Guild: {Context.Guild.Name}"));

				await message.SendMessageAsync("", false, embed);
				embed.Description = $"You have sent a message to {me}. He will read the message soon.";
				await Context.Channel.SendMessageAsync("", false, embed);
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Prints the bots changelog inside the chat
		/// </summary>
		/// <returns></returns>
		[Command("changelog", RunMode = RunMode.Async)]
		[Remarks("Returns Ricas Changelog which includes her version")]
		public async Task Changelog()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				await ReplyAsync(
				System.IO.File.ReadAllText(@"changelog.txt"));
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}


		// Database stuff

		/// <summary>
		/// Returns data stored into the database
		/// </summary>
		/// <param name="user">The user.</param>
	
		/// <returns></returns>
		[Command("status", RunMode = RunMode.Async)]
		[Alias("s")]
		[Remarks("Retrieves data about a user from the Database")]
		public async Task dbSay([Remainder] IUser user = null)
		{

			if (BotCooldown.isCooldownRunning == false)
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
									 + ":small_blue_diamond:" + "Current custom rank: " + tableName.FirstOrDefault().Rank + "\n");

				await Context.Channel.SendMessageAsync("", false, embed);
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}
			

		/// <summary>
		/// Registers you inside the Database
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("enterDb", RunMode = RunMode.Async)]
		[Remarks("Enters you into Ricas Database")]
		public async Task dbEnter([Remainder] IUser user = null)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (user == null)
				{
					user = Context.User;
					Database.EnterUser(user);
					await ReplyAsync("You should be entered now. If not, then hell...");
					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}
				


		// Database Awards, UwU

		/// <summary>
		/// When i want to award someone with some prestige
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="tokens">The tokens.</param>
		/// <returns></returns>
		[Command("award", RunMode = RunMode.Async)]
		[Remarks("Award someone with some tokens, Woo!")]
		public async Task Award(SocketGuildUser user, [Remainder] uint tokens)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (tokens > 50)
				{
					await ReplyAsync(Context.User.Mention +
									 ", Woah there. The amount you entered was too high to handle.\nKeep it at least below or equal to 50!");
					await BotCooldown.Cooldown();
				}
				else
				{
					Database.ChangeTokens(user, tokens);
					await ReplyAsync(user.Mention + ", you were awarded with " + tokens + " tokens!");
					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Random cat!
		/// </summary>
		/// <returns></returns>
		[Command("cat", RunMode = RunMode.Async)]
		[Remarks("Sends you a random cat.")]
		public async Task Cat()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				Console.WriteLine("Making API Call...");
				using (var client = new HttpClient(new HttpClientHandler
				{
					AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
				}))
				{
					string websiteUrl = "http://random.cat/meow";
					client.BaseAddress = new Uri(websiteUrl);

					HttpResponseMessage res = client.GetAsync("").Result;
					res.EnsureSuccessStatusCode();

					string result = await res.Content.ReadAsStringAsync();
					var json = JObject.Parse(result);

					string CatImage = json["file"].ToString();

					await ReplyAsync(CatImage);
					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// Gets the best urban definition based on the term the user has given.
		/// </summary>
		/// <param name="term">The term.</param>
		/// <returns></returns>
		[Command("ud", RunMode = RunMode.Async)]
		[Remarks("Returns an Urban Dictionary defintion")]
		public async Task Urban([Remainder] string term = null)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				UrbanService client = new UrbanService();
				var data = await client.Data($"{term}");
				var tUp = data.List[0].ThumbsUp;
				var tDown = data.List[0].ThumbsDown;
				var def = data.List[0].Definition;
				var ex = data.List[0].Example;

				var embed = new EmbedBuilder()
				{
					Color = new Color(60, 133, 150)
				};

				embed.Title = $"Urban Definiton for {term}";
				embed.Description =
				($"{def}\n------------\nExample:\n{ex}\n-----------\nThis Urban Defintion has received {tUp} :thumbsup: and {tDown} :thumbsdown:"
				);

				await ReplyAsync("", false, embed: embed);
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}


		/// <summary>
		/// If someone actually is going to donate.
		/// </summary>
		/// <returns></returns>
		[Command("donate", RunMode = RunMode.Async)]
		[Remarks("If you want to show your support, then do it with this!")]
		public async Task Donate()
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				await ReplyAsync(
			"If you want to show your support to EnK_ for making me, you can do it over paypal!\nAny amount is accepted (Except an amount of 0) and will greatly help him in keeping this project running!\nYou can donate to him here: https://www.paypal.me/zi8tx");
				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		/// This is the subclass which is handled as a submodule of the public module and which handles all poll related things!
		/// </summary>
		/// <seealso cref="Discord.Commands.ModuleBase" />
		[Group("poll")]
		public class Poll : ModuleBase
		{
			/// <summary>
			/// This is the string which helps us checking if a poll is currently running. If it says yes, then there can't be another poll made until it says no again
			/// </summary>
			private static string _isAPollRunning = "no";

			/// <summary>
			/// Security-Measure for preventing that other users end your poll
			/// </summary>
			private static string _userMadePoll = "";

			/// <summary>
			/// Starts the poll.
			/// </summary>
			/// <param name="question">The question.</param>
			/// <param name="user">The user.</param>
			/// <returns></returns>
			[Command("start", RunMode = RunMode.Async)]
			[Remarks("Starts a poll")]
			public async Task StartPoll(string question, [Remainder] IUser user)
			{

				if (BotCooldown.isCooldownRunning == false)
				{
					if (_isAPollRunning.Equals("yes"))
					{
						await ReplyAsync("You cannot start a poll at the moment! Please wait until the current poll is over");
						await BotCooldown.Cooldown();
					}

					if (user == null)
					{
						user = Context.User;
						_isAPollRunning = "yes";
						_userMadePoll = user.Username;

						var embed = new EmbedBuilder()
						{
							Color = new Color(56, 193, 25)
						};

						Database.EnterPoll(question, user);

						embed.Title = $"{Context.User.Username} has started a poll!";

						var tableName = Database.GetPoll();

						embed.Description = ($"{Context.User.Username} has started a poll" + "\n\n" +
											 ":arrow_right: Question: " + tableName.FirstOrDefault().Question + "\n" +
											 ":arrow_right: Votes for yes: " + tableName.FirstOrDefault().YesVotes + "\n" +
											 ":arrow_right: Votes for no: " + tableName.FirstOrDefault().NoVotes);

						await Context.Channel.SendMessageAsync("", false, embed);
						await BotCooldown.Cooldown();
					}
					else // If there was a user mentioned
					{
						_isAPollRunning = "yes";

						var embed = new EmbedBuilder()
						{
							Color = new Color(56, 193, 25)
						};

						Database.EnterPoll(question, user);

						embed.Title = $"{Context.User.Username} has started a poll!";

						var tableName = Database.GetPoll();

						embed.Description = ($"{Context.User.Username} has started a poll" + "\n\n" +
											 ":arrow_right: Question: " + tableName.FirstOrDefault().Question + "\n" +
											 ":arrow_right: Votes for yes: " + tableName.FirstOrDefault().YesVotes + "\n" +
											 ":arrow_right: Votes for no: " + tableName.FirstOrDefault().NoVotes);

						await Context.Channel.SendMessageAsync("", false, embed);
						await BotCooldown.Cooldown();
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}


			/// <summary>
			/// Votes on the poll with a given specified decision.
			/// </summary>
			/// <param name="decision">The decision.</param>
			/// <param name="user">The user.</param>
			/// <returns></returns>
			[Command("vote", RunMode = RunMode.Async)]
			[Remarks("Leave your vote here!")]
			public async Task Vote(string decision, [Remainder] IUser user)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (_isAPollRunning.Equals("no"))
					{
						await ReplyAsync("You cannot vote on a poll which is not running!");
						await BotCooldown.Cooldown();
					}

					if (user == null)
					{
						user = Context.User;
						if (decision.Equals("yes") || decision.Equals("Yes") || decision.Equals("YES"))
						{
							Database.CheckExistingVoteByUser(user);
							Database.AddYesToPoll();
							Database.EnterUserVote(user, 1);
							await ReplyAsync("You successfully voted for yes on the current poll!");
							await BotCooldown.Cooldown();
						}
						else if (decision.Equals("no") || decision.Equals("No") || decision.Equals("NO"))
						{
							Database.CheckExistingVoteByUser(user);
							Database.AddNoToPoll();
							Database.EnterUserVote(user, 0);
							await ReplyAsync("You successfully voted for no on the current poll!");
							await BotCooldown.Cooldown();
						}
						else
						{
							await ReplyAsync("Invalid answer!");
							await BotCooldown.Cooldown();
						}
					}
					else // If there was a user mentioned
					{
						if (decision.Equals("yes") || decision.Equals("Yes") || decision.Equals("YES"))
						{
							Database.CheckExistingVoteByUser(user);
							Database.AddYesToPoll();
							Database.EnterUserVote(user, 1);
							await ReplyAsync("You successfully voted for yes on the current poll!");
							await BotCooldown.Cooldown();
						}
						else if (decision.Equals("no") || decision.Equals("No") || decision.Equals("NO"))
						{
							Database.CheckExistingVoteByUser(user);
							Database.AddNoToPoll();
							Database.EnterUserVote(user, 0);
							await ReplyAsync("You successfully voted for no on the current poll!");
							await BotCooldown.Cooldown();
						}
						else
						{
							await ReplyAsync("Invalid answer!");
							await BotCooldown.Cooldown();
						}
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// Ends the poll.
			/// </summary>
			/// <param name="user">The user</param>
			/// <returns></returns>
			[Command("end", RunMode = RunMode.Async)]
			[Remarks("Ends your poll")]
			public async Task EndPoll([Remainder] IUser user)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (_isAPollRunning.Equals("no"))
					{
						await ReplyAsync("You cannot end a poll if there is nothing to end!");
						await BotCooldown.Cooldown();
					}

					if (user == null)
					{
						user = Context.User;
						if (_userMadePoll != user.Username)
						{
							await ReplyAsync("You cannot end a poll which wasn't opened by you!");
							await BotCooldown.Cooldown();
						}
						else
						{
							var embed = new EmbedBuilder()
							{
								Color = new Color(56, 193, 25)
							};

							embed.Title = $"{Context.User.Username} has ended their poll!";

							var tableName = Database.GetPoll();

							embed.Description = ($"{Context.User.Username} has ended their poll" + "\n\n" +
												 "Here is the result:" + "\n" +
												 $":arrow_right: Question: {tableName.FirstOrDefault().Question}" + "\n" +
												 $":arrow_right: Votes for yes: {tableName.FirstOrDefault().YesVotes}" + "\n" +
												 $":arrow_right: Votes for no: {tableName.FirstOrDefault().NoVotes}");

							await Context.Channel.SendMessageAsync("", false, embed);

							_isAPollRunning = "no";

							Database.DeletePoll();
							Database.DeleteUserInVotePool();

							_userMadePoll = "";

							await BotCooldown.Cooldown();
						}
					}
					else // If there was an user mentioned
					{
						if (_userMadePoll != user.Username)
						{
							await ReplyAsync("You cannot end a poll which wasn't opened by you!");
							await BotCooldown.Cooldown();
						}
						else
						{
							var embed = new EmbedBuilder()
							{
								Color = new Color(56, 193, 25)
							};

							embed.Title = $"{Context.User.Username} has ended their poll!";

							var tableName = Database.GetPoll();

							embed.Description = ($"{Context.User.Username} has ended their poll" + "\n\n" +
												 "Here is the result:" + "\n" +
												 $":arrow_right: Question: {tableName.FirstOrDefault().Question}" + "\n" +
												 $":arrow_right: Votes for yes: {tableName.FirstOrDefault().YesVotes}" + "\n" +
												 $":arrow_right: Votes for no: {tableName.FirstOrDefault().NoVotes}");

							await Context.Channel.SendMessageAsync("", false, embed);

							_isAPollRunning = "no";

							Database.DeletePoll();
							Database.DeleteUserInVotePool();

							_userMadePoll = "";

							await BotCooldown.Cooldown();
						}
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}
		}


		// Economy related.

		/// <summary>
		/// This subclass is handled as a submodule of the public module and handles all economical related things
		/// </summary>
		/// <seealso cref="Discord.Commands.ModuleBase" />
		[Group("Currency")]
		public class Economy : ModuleBase
		{
			/// <summary>
			/// First you gotta open a bank account before you get any money.
			/// </summary>
			/// <returns></returns>
			[Command("openbank", RunMode = RunMode.Async)]
			[Remarks("Opens your bank account! Woah!")]
			public async Task bankC()
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					var application = await Context.Client.GetApplicationInfoAsync();
					var auth = new EmbedAuthorBuilder();

					var result = Database.CheckMoneyExistingUser(Context.User);
					if (result.Count() <= 0)
					{
						Database.cBank(Context.User);

						var embed = new EmbedBuilder()
						{
							Color = new Color(29, 140, 209),
							Author = auth
						};

						embed.Title = $"{Context.User.Username} has opened a bank-account!";
						embed.Description = $"\n:dollar: **Welcome to the bank!** :\n\n**Bank: Rica Bank**\n";

						await ReplyAsync("", false, embed.Build());
						await BotCooldown.Cooldown();
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// Returns your balance
			/// </summary>
			/// <returns></returns>
			[Command("balance", RunMode = RunMode.Async)]
			[Remarks("Returns your current balance")]
			public async Task MoneyOl()
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					var application = await Context.Client.GetApplicationInfoAsync();
					var auth = new EmbedAuthorBuilder();

					var moneydiscord = Database.GetUserMoney(Context.User);

					var embed = new EmbedBuilder()
					{
						Color = new Color(29, 140, 209),
						Author = auth
					};

					embed.Title = $"{Context.User.Username}'s Balance";
					embed.Description = $"\n:dollar: **Balance**:\n\n**{moneydiscord.FirstOrDefault().Money}**\n";

					await ReplyAsync("", false, embed.Build());
					await BotCooldown.Cooldown();
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// When i want to add money to someones account
			/// </summary>
			/// <param name="user">The user.</param>
			/// <param name="money">The money.</param>
			/// <returns></returns>
			[Command("givemoney", RunMode = RunMode.Async)]
			[Remarks("Adds money to a user.")]
			public async Task AddMoney(SocketGuildUser user, [Remainder] int money)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (Context.User.Id == 112559794543468544)
					{
						Database.UpdateMoney(user, money);
						await ReplyAsync($"Gave {money} to {user.Username}!");
						await BotCooldown.Cooldown();
					}
					else
					{
						await ReplyAsync("Only my master can add money to others...");
						await BotCooldown.Cooldown();
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// Part 1 of the payment process
			/// </summary>
			/// <param name="user">The user.</param>
			/// <param name="moneyToStore">The money to store.</param>
			/// <returns></returns>
			[Command("store", RunMode = RunMode.Async)]
			[Remarks("Part of the payment process.")]
			public async Task StoreMoney(int moneyToStore, [Remainder] IUser user = null)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (user == null)
					{
						user = Context.User;

						var _user = Database.CheckMoneyExistingUser(user);
						var embedAuthor = new EmbedAuthorBuilder();

						if (user == null)
						{
							user = Context.User;
						}

						if (_user.Count <= 0)
						{
							Database.cBank(user);
						}
						else
						{
							Database.StoreMoney(user, moneyToStore);

							var embed = new EmbedBuilder()
							{
								Color = new Color(0, 0, 255),
								Author = embedAuthor
							};

							embed.Title = $"{user} has stored {moneyToStore} Dollars into their vault";
							embed.Description = "You may now send the amount of money you stored away to the user you want to pay.";

							await ReplyAsync("", false, embed: embed);
							await BotCooldown.Cooldown();
						}
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// Part 2 of the payment process
			/// </summary>
			/// <param name="payUser">The pay user.</param>
			/// <param name="recieveUser">The recieve user.</param>
			/// <param name="money">The money.</param>
			/// <returns></returns>
			[Command("pay", RunMode = RunMode.Async)]
			[Remarks("Pays the user with stored money")]
			public async Task PayMoney(IUser recieveUser, int money, [Remainder] IUser payUser = null)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (payUser == null)
					{
						payUser = Context.User;
						var moneyDiscord = Database.GetUserMoney(Context.User);

						if (moneyDiscord.FirstOrDefault().StoreMoney < money)
						{
							await ReplyAsync("You can't pay more than you have stored away, my friend");
							await BotCooldown.Cooldown();
						}
						else
						{
							Database.PayMoney1(payUser, money);
							Database.PayMoney2(recieveUser, money);
							await ReplyAsync($"Successfully paid {recieveUser} {money} Dollars!");
							await BotCooldown.Cooldown();
						}
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// Dailies this instance.
			/// </summary>
			/// <returns></returns>
			[Command("daily", RunMode = RunMode.Async)]
			[Remarks("Daily tokens and money! Yey!")]
			public async Task Daily()
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					var user = Context.User;
					var result = Database.CheckExistingUser(user);

					if (result.Count() <= 0)
						Database.EnterUser(user);

					var discord = Database.GetUserStatus(user);

					DateTime now = DateTime.Now;
					DateTime daily = discord.FirstOrDefault().Daily;
					int diff1 = DateTime.Compare(daily, now);

					if ((discord.FirstOrDefault().Daily.ToString() == "0001-01-01 00:00:00") ||
						(daily.DayOfYear < now.DayOfYear && diff1 < 0 || diff1 >= 0))
					{
						Database.ChangeDaily(user);
						int Money = 400;
						uint Tokens = 250;
						Database.AddMoney2(user, Money);
						Database.ChangeTokens(user, Tokens);
						await ReplyAsync($"You have received your daily {Money} Dollars and {Tokens} Prestige-Tokens!");
						await BotCooldown.Cooldown();
					}
					else
					{
						TimeSpan diff2 = now - daily;
						TimeSpan di = new TimeSpan(23 - diff2.Hours, 60 - diff2.Minutes, 60 - diff2.Seconds);
						await ReplyAsync($"Your daily renews in {di}.");
						await BotCooldown.Cooldown();
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}

			/// <summary>
			/// This class is for the games based on economical features
			/// </summary>
			/// <seealso cref="Discord.Commands.ModuleBase" />
			[Group("Gamble")]
			public class EcoGames : ModuleBase
			{

				/// <summary>
				/// Either you win or you lose.
				/// </summary>
				/// <param name="bet">The bet.</param>
				/// <returns></returns>
				[Command("bet", RunMode = RunMode.Async)]
				[Remarks("Place your bets and roll the dice!")]
				public async Task betCmd(int bet)
				{
					if (BotCooldown.isCooldownRunning == false)
					{
						var moneydiscord = Database.GetUserMoney(Context.User);

						if (moneydiscord.FirstOrDefault().Money < bet)
						{
							await ReplyAsync("You do not have enough money to bet this high");
							await BotCooldown.Cooldown();
						}
						else
						{
							Random rand = new Random();
							Random rand2 = new Random();

							int userRoll = rand2.Next(1, 6);
							int rolled = rand.Next(1, 9);

							if (userRoll == rolled)
							{
								Database.UpdateMoney(Context.User, bet);
								await ReplyAsync($"Congrats {Context.User.Username}!, you have made ${bet} off rolling a **{userRoll}**!");
								await BotCooldown.Cooldown();
							}
							else
							{
								int betRemove = -bet;

								Database.UpdateMoney(Context.User, betRemove);
								await ReplyAsync($"Sorry **{Context.User.Username}**, you lost ${bet} off rolling a **{userRoll}**!");
								await BotCooldown.Cooldown();
							}
						}
					}
				}
			}
		}
	}
}