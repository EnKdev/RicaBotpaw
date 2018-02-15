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
using RicaBotpaw.Logging;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using RicaBotpaw.Config;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ImageSharp.Formats;
using Newtonsoft.Json.Converters;
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
		private int modEnable;

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

		private async Task CheckEnabledPublicModule([Remainder] IGuild g = null)
		{
			if (g == null)
			{
				g = Context.Guild;

				if (!File.Exists($"./serv_configs/{g.Id.ToString()}.rconf"))
				{
					await ReplyAsync(ModStrings.GuildNoConfigFile);
				}
				else
				{
					using (StreamReader file = File.OpenText($"./serv_configs/{g.Id.ToString()}.rconf"))
					{
						JsonSerializer ser = new JsonSerializer();
						Config.Modules mods = (Config.Modules) ser.Deserialize(file, typeof(Config.Modules));

						if (mods.Guild != g.Id)
						{
							await ReplyAsync(
								"Specified Guild ID doesn't match saved Guild ID in config file."); // This should actually never happen
						}
						else if (mods.ModPub == 1)
						{
							modEnable = 1;
						}
						else // If it is not 1, but 2 or higher than 1 or even 0, then the module is disabled by default
						{
							modEnable = 0;
						}
					}
				}
			}
		}

		/// <summary>
		/// Sometimes you need help...
		/// </summary>
		/// <returns></returns>
		[Command("help", RunMode = RunMode.Async)]
		[Remarks("Shows a list of all available commands per module")]
		public async Task HelpAsync()
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
			}
		}

		[Command("mhelp", RunMode = RunMode.Async)]
		[Alias("m")]
		[Remarks("Shows specific information about the modules.")]
		public async Task ModuleHelp()
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
			}
		}

		[Command("stop", RunMode = RunMode.Async)]
		[Remarks("Stops the bot. For updating processes.")]
		public async Task StopTask() // Exempt from the configurations since it is a dev command
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

		[Command("dstop", RunMode = RunMode.Async)]
		[Remarks("The dev stop")]
		public async Task
			DevStop(string logId, int caseIdentifier,
				[Remainder] string reason = null) // Exempt from the configurations since it is a dev command
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (!(Context.User.Id == 112559794543468544))
				{
					await Context.Channel.SendMessageAsync(
						"You are unable to create a devStop Log. Only EnK_ can do that action.");
					await BotCooldown.Cooldown();
				}
				else
				{
					var logFileName1 = $"{logId}_" + $"{caseIdentifier}";
					var logFileName2 = "_devStop";
					var logFileName = logFileName1 + logFileName2;

					StopReasonJSON sReason = new StopReasonJSON
					{
						Case = caseIdentifier,
						LogId = $"{logId}",
						R = $"{reason}",
						CIDComment = "Case Identifier Help: 0 -> Dev Update, 1 -> Severe Issue"
					};

					StopReasonXML sReason1 = new StopReasonXML
					{
						Case = caseIdentifier,
						CaseHelp = "Case Identifier Help: 0 = Dev Update, 1 = Severe Issue",
						LogId = $"{logId}",
						R = $"{reason}"
					};

					// This saves the devstop as a rblog file which is actually just a simple json file tbh.
					using (StreamWriter file = File.CreateText($"./logs/{logFileName}.rblog"))
					{
						JsonSerializer serializer = new JsonSerializer();
						serializer.Serialize(file, sReason);
						await ReplyAsync($"LogFile {logFileName}.rblog created in /logs");
					}

					// Backup purposes. A XML File is also going to be generated
					using (StreamWriter file1 = File.CreateText($"./logs/{logFileName}.rblog.xml"))
					{
						XmlSerializer ser1 = new XmlSerializer(typeof(StopReasonXML));
						ser1.Serialize(file1, sReason1);
						await ReplyAsync($"XML LogFile {logFileName}.rblog.xml created in /logs");
					}
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
			var gld = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(gld);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					using (var process = Process.GetCurrentProcess())
					{
						var embed = new EmbedBuilder();
						var application = await Context.Client.GetApplicationInfoAsync();
						var weekCount = 0;
						var dayCount = 0;
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

								if (time.Days > 7)
								{
									weekCount++;
									sb.Append($"{weekCount}w ");
									dayCount = 0;
								}
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
						$"Username: **{D}**\n" +
						$"Discriminator: **{A}**\n" +
						$"User ID: **{T}**\n" +
						$"Created at: **{S}**\n" +
						$"Current Status: **{C}**\n" +
						$"Joined server at: **{CC}**\n" +
						$"Playing: **{O}**";

					await ReplyAsync("", false, embed.Build());
					await BotCooldown.Cooldown();
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
		public async Task
			dm([Remainder] string dm) // Exempt from the configuration as this is an essential feature to contact the bot dev.
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
		public async Task
			Changelog() // Exempt from the configuration as this is an essential update command containing every change
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnabledPublicModule(g);

			if (modEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PublicNotEnabled);
			}
		}
	}
}