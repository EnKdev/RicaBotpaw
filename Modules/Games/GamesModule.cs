using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ImageSharp;
using RicaBotpaw.Modules.Data;
using SixLabors.Primitives;
using Newtonsoft.Json;
using RicaBotpaw.Logging;

namespace RicaBotpaw.Modules.Games
{ 
	/// <summary>
	///     No bot without games.
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	[Remarks("This is the games module which contains all of the bots games.")]
	public class Games : ModuleBase
	{
		private int modEnable;
		private int gNoticeSent;

		/// <summary>
		///     The service
		/// </summary>
		private CommandService _service;

		// Coin-Flip
		private readonly string[] coinSides =
		{
			"Tails",
			"Heads"
		};
		// 8ball

		/// <summary>
		///     The eight ball predicts
		/// </summary>
		private readonly string[] eightBallPredicts =
		{
			"It is very unlikely.",
			"I don't think so...",
			"Yes!",
			"I don't know",
			"No.",
			"It would be a come and go",
			"Definitely",
			"Care to elaborate?",
			"If you want to...",
			"My sources say yes",
			"My sources say no",
			"Maybe in another life",
			"I am not a prediction",
			"Reply hazy, try again!",
			"Obviously no",
			"Obviously yes!"
		};

		private readonly Random rand = new Random();

		private readonly string[] rps =
		{
			"Rock",
			"Paper",
			"Scissors"
		};

		/// <summary>
		///     This initializes the GamesModule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Games(CommandService service)
		{
			_service = service;
		}

		private async Task CheckEnabledGameModule([Remainder] IGuild g = null)
		{
			if (g == null) g = Context.Guild;

			if (!File.Exists($"./serv_configs/{g.Id.ToString()}_config.rconf"))
			{
				await ReplyAsync(ModStrings.GuildNoConfigFile);
				gNoticeSent = 1;
				return;
			}

			var fileText = File.ReadAllText($"./serv_configs/{g.Id.ToString()}_config.rconf");
			var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText);

			if (mods.Guild != g.Id)
			{
				await ReplyAsync(
					"Specified Guild ID doesn't match saved Guild ID in config file."); // This should actually never happen
				return;
			}
			if (mods.ModGame == 1)
			{
				modEnable = 1;
				return;
			}
			modEnable = 0;
		}

		/// <summary>
		/// Gives you a prediction
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		[Command("8ball", RunMode = RunMode.Async)]
		[Remarks("Gives a prediction")]
		public async Task EightBall([Remainder] string input)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledGameModule(g);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input.Equals("loop"))
					{
						await ReplyAsync(
							"The prediction is never the prediction is never the prediction is never the prediction is never the prediction is never the prediction"); // The stanley parable reference
						await BotCooldown.Cooldown();
					}
					else if (input.Equals("force"))
					{
						await ReplyAsync("These are not the droids you are looking for."); // Star wars
						await BotCooldown.Cooldown();
					}
					else if (input.Equals("chicken"))
					{
						await ReplyAsync("Winner Winner Chicken-Dinner!"); // PUBG
						await BotCooldown.Cooldown();
					}
					else
					{
						var randomIndex = rand.Next(eightBallPredicts.Length);
						var text = eightBallPredicts[randomIndex];
						await ReplyAsync(Context.User.Mention + ", " + text);
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
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.GamesNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}


		/// <summary>
		/// RPSs the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		[Command("rps", RunMode = RunMode.Async)]
		[Remarks("Rock, Paper, Scissors")]
		public async Task RPS([Remainder] string input)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledGameModule(g);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input.Equals("Dwayne 'The Rock' Johnson"))
					{
						await ReplyAsync("Is that a choice? I am confused.");
						await BotCooldown.Cooldown();
					}
					else if (input.Equals("Donald Trump"))
					{
						await ReplyAsync("Have a drawing donald made!");
						await Context.Channel.SendFileAsync(
							@".\images\DonaldDraws1511338722535.gif");
						await BotCooldown.Cooldown();
					}
					else
					{
						var randIdx = rand.Next(rps.Length);
						var choice = rps[randIdx];

						// Rock
						if (choice.Equals("Rock") && input.Equals("Rock"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nDRAW!");
							await BotCooldown.Cooldown();
						}
						else if (choice.Equals("Rock") && input.Equals("Scissors"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nI WIN!");
							await BotCooldown.Cooldown();
						}
						else if (choice.Equals("Rock") && input.Equals("Paper"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nYOU WON!");
							await BotCooldown.Cooldown();
						}

						// Paper
						else if (choice.Equals("Paper") && input.Equals("Paper"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nDRAW!");
							await BotCooldown.Cooldown();
						}
						else if (choice.Equals("Paper") && input.Equals("Rock"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nI WIN!");
							await BotCooldown.Cooldown();
						}
						else if (choice.Equals("Paper") && input.Equals("Scissors"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nYOU WON!");
							await BotCooldown.Cooldown();
						}

						// Scissors
						else if (choice.Equals("Scissors") && input.Equals("Scissors"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nDRAW!");
							await BotCooldown.Cooldown();
						}
						else if (choice.Equals("Scissors") && input.Equals("Paper"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nI WIN!");
							await BotCooldown.Cooldown();
						}
						else if (choice.Equals("Scissors") && input.Equals("Rock"))
						{
							await ReplyAsync($"You chose {input}, i chose {choice}\nYOU WON!");
							await BotCooldown.Cooldown();
						}

						// Lel
						else if (choice.Equals("Rock") && input.Equals("Nuke") || choice.Equals("Paper") && input.Equals("Nuke") ||
								 choice.Equals("Scissors") && input.Equals("Nuke"))
						{
							await ReplyAsync($"You chose a nuke. I chose {choice}. Guess it is not fun to play against Kim-Jong-Un");
							await Context.Channel.SendFileAsync(@".\images\nuke.gif");
							await BotCooldown.Cooldown();
						}

						// If no input matches the required choices (Rock, Paper, Scissors, Nuke, DTRJ, Donald)
						else if (input != "Rock" || input != "Scissors" || input != "Paper" || input != "Nuke" ||
								 input != "Dwayne 'The Rock' Johnson")
						{
							await ReplyAsync("Invalid input detected. Try again with a valid choice.");
							await BotCooldown.Cooldown();
						}
					}
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.GamesNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}


		/// <summary>
		/// Flips a coin.
		/// </summary>
		/// <param name="choice">The choice.</param>
		/// <returns></returns>
		[Command("coinflip", RunMode = RunMode.Async)]
		[Remarks("Flips a coin")]
		public async Task CoinFlip(string choice)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledGameModule(g);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					var randIdx = rand.Next(coinSides.Length);
					var side = coinSides[randIdx];

					if (choice.Equals("Heads") || choice.Equals("heads") && side.Equals("Tails"))
					{
						await ReplyAsync("You lost!\nYou chose Heads, and the coin landed on Tails");
						await BotCooldown.Cooldown();
					}
					else if (choice.Equals("Tails") || choice.Equals("tails") && side.Equals("Heads"))
					{
						await ReplyAsync("You lost!\nYou chose Tails, and the coin landed on Heads");
						await BotCooldown.Cooldown();
					}
					else if (choice.Equals("Heads") || choice.Equals("heads") && side.Equals("Heads"))
					{
						await ReplyAsync("You won!\nYou chose Heads, and the coin landed on Heads");
						await BotCooldown.Cooldown();
					}
					else if (choice.Equals("Tails") || choice.Equals("tails") && side.Equals("Tails"))
					{
						await ReplyAsync("You won!\nYou chose Tails, and the coin landed on Tails");
						await BotCooldown.Cooldown();
					}
					else if (choice != "Heads" || choice != "heads" || choice != "Tails" || choice != "tails")
					{
						await ReplyAsync("Invalid input detected. Please use only heads or tails");
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
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.GamesNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		/// <summary>
		///     Spinnedy spin!
		/// </summary>
		/// <param name="_user1">The user1.</param>
		/// <param name="_user2">The user2.</param>
		/// <param name="_user3">The user3.</param>
		/// <param name="_user4">The user4.</param>
		/// <returns></returns>
		[Command("spin", RunMode = RunMode.Async)]
		[Remarks("If you can't decide who to pick, use this.")]
		public async Task ArrowSpinAsync(IGuildUser _user1 = null, IGuildUser _user2 = null, IGuildUser _user3 = null,
			IGuildUser _user4 = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledGameModule(g);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					var rand = new Random();
					IVoiceChannel channel2 = null;

					var randomUsers = new IGuildUser[4];

					try
					{
						channel2 = (Context.User as IVoiceState).VoiceChannel;
						var vc = channel2 as SocketVoiceChannel;
						var voiceUsers = vc.Users;

						randomUsers[0] = voiceUsers.ElementAt(0);
						randomUsers[1] = voiceUsers.ElementAt(1);
						randomUsers[2] = voiceUsers.ElementAt(2);
						randomUsers[3] = voiceUsers.ElementAt(3);
					}
					catch
					{
						var users = await Context.Guild.GetUsersAsync();
						randomUsers[0] = users.ElementAt(rand.Next(0, users.Count));
						randomUsers[1] = users.ElementAt(rand.Next(0, users.Count));
						randomUsers[2] = users.ElementAt(rand.Next(0, users.Count));
						randomUsers[3] = users.ElementAt(rand.Next(0, users.Count));
					}

					if (_user1 != null) randomUsers[0] = _user1;
					if (_user2 != null) randomUsers[1] = _user2;
					if (_user3 != null) randomUsers[2] = _user3;
					if (_user4 != null) randomUsers[3] = _user4;

					var core = new ImageCore.ImageCore();

					var user1 = await core.StartStreamAsync(randomUsers[0]);
					var user2 = await core.StartStreamAsync(randomUsers[1]);
					var user3 = await core.StartStreamAsync(randomUsers[2]);
					var user4 = await core.StartStreamAsync(randomUsers[3]);

					var arrow = await core.StartStreamAsync(path: "./images/arrow.png");

					var finalImg = new Image<Rgba32>(500, 500);

					var size250 = new Size(250, 250);
					var size500 = new Size(500, 500);

					user1.Resize(size250);
					user2.Resize(size250);
					user3.Resize(size250);
					user4.Resize(size250);

					finalImg.DrawImage(user1, 1f, size250, new Point(0, 0));
					finalImg.DrawImage(user2, 1f, size250, new Point(250, 0));
					finalImg.DrawImage(user3, 1f, size250, new Point(0, 250));
					finalImg.DrawImage(user4, 1f, size250, new Point(250, 250));

					float dir = rand.Next(0, 360);
					string winner = null;

					if (dir > 270 && dir < 360) winner = randomUsers[0].Username;
					if (dir > 0 && dir < 90) winner = randomUsers[1].Username;
					if (dir > 90 && dir < 180) winner = randomUsers[3].Username;
					if (dir > 180 && dir < 270) winner = randomUsers[2].Username;

					if (winner == null) winner = "No one, try again";

					arrow.Rotate(dir);

					finalImg.DrawImage(arrow, 1f, size500, new Point(0, 0));

					await core.StopStreamAsync(Context.Message, finalImg);

					await Context.Channel.SendMessageAsync($"The spinner is pointing at **{winner}**");

					await BotCooldown.Cooldown();
				}
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.GamesNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}
	}
}