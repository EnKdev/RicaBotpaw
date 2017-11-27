using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using ImageSharp;
using ImageSharp.Drawing.Pens;
using ImageSharp.Drawing.Brushes;
using ImageSharp.Processing;
using SixLabors.Fonts;
using SixLabors.Primitives;
using System.Numerics;
using ImageSharp.Dithering;
using RicaBotpaw.ImageCore;

namespace RicaBotpaw.Modules.Games
{
	/// <summary>
	/// No bot without games.
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	public class Games : ModuleBase
	{
		// 8ball

		/// <summary>
		/// The eight ball predicts
		/// </summary>
		string[] eightBallPredicts = new string[]
		{
			"It is very unlikely.",
			"I don't think so...",
			"Yes!",
			"I don't know",
			"No.",
			"It would be a come and go",
			"Definitly",
			"Care to elaborate?",
			"If you want to...",
			"My sources say yes",
			"My sources say no",
			"Maybe in another life",
			"I am not a prediction"
		};

		string[] rps = new string[]
		{
			"Rock",
			"Paper",
			"Scissors"
		};

		Random rand = new Random();

		/// <summary>
		/// The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		/// This initializes the GamesModule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Games(CommandService service)
		{
			_service = service;
		}

		/// <summary>
		/// Gives you a prediction
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		[Command("8ball")]
		[Remarks("Gives a prediction")]
		public async Task EightBall([Remainder] string input)
		{
			if (input.Equals("loop"))
			{
				await ReplyAsync("The prediction is never the prediction is never the prediction is never the prediction is never the prediction is never the prediction"); // The stanley parable reference
			}
			else if(input.Equals("force"))
			{
				await ReplyAsync("These are not the droids you are looking for."); // Star wars
			}
			else if(input.Equals("chicken"))
			{
				await ReplyAsync("Winner Winner Chicken-Dinner!"); // PUBG
			}
			else
			{
				int randomIndex = rand.Next(eightBallPredicts.Length);
				string text = eightBallPredicts[randomIndex];
				await ReplyAsync(Context.User.Mention + ", " + text);
			}
		}

		/// <summary>
		/// RPSs the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <returns></returns>
		[Command("rps")]
		[Remarks("Rock, Paper, Scissors")]
		public async Task RPS([Remainder] string input)
		{
			if (input.Equals("Dwayne 'The Rock' Johnson"))
			{
				await ReplyAsync("Is that a choice? I am confused.");
			}
			else
			{
				int randIdx = rand.Next(rps.Length);
				string choice = rps[randIdx];
				
				// Rock
				if (choice.Equals("Rock") && input.Equals("Rock"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nDRAW!");
				}
				else if (choice.Equals("Rock") && input.Equals("Scissors"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nI WIN!");
				}
				else if (choice.Equals("Rock") && input.Equals("Paper"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nYOU WON!");
				}

				// Paper
				else if (choice.Equals("Paper") && input.Equals("Paper"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nDRAW!");
				}
				else if (choice.Equals("Paper") && input.Equals("Rock"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nI WIN!");
				}
				else if (choice.Equals("Paper") && input.Equals("Scissors"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nYOU WON!");
				}

				// Scissors
				else if (choice.Equals("Scissors") && input.Equals("Scissors"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nDRAW!");
				}
				else if (choice.Equals("Scissors") && input.Equals("Paper"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nI WIN!");
				}
				else if (choice.Equals("Scissors") && input.Equals("Rock"))
				{
					await ReplyAsync($"You chose {input}, i chose {choice}\nYOU WON!");
				}

				// Lel
				else if (choice.Equals("Rock") && input.Equals("Nuke") || choice.Equals("Paper") && input.Equals("Nuke") || choice.Equals("Scissors") && input.Equals("Nuke"))
				{
					await ReplyAsync($"You chose a nuke. I chose {choice}. Guess it is not fun to play against Kim-Jong-Un");
					await Context.Channel.SendFileAsync(@"C:\Users\LordaS\Desktop\Rica Botpaw\RicaBotpaw\images\nuke.gif");
				}

				// If no input matches the required choices (Rock, Paper, Scissors, Nuke, DTRJ)
				else if (input != ("Rock") || input != ("Scissors") || input != ("Paper") || input != ("Nuke") || input != ("Dwayne 'The Rock' Johnson"))
				{
					await ReplyAsync($"Invalid input detected. Try again with a valid choice.");
				}
			}
		}

		// Turn-based fight

		static string player1;
		static string player2;
		static string whosTurn;
		static string whoWaits;
		static string placeHolder;
		static int health1 = 100;
		static int health2 = 100;
		static string SwitchCaseString = "nofight";

		/// <summary>
		/// Turn-Based-Fights FTW
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("fight")]
		[Alias("battle")]
		[Remarks("Starts a fight with the @mention user (example: ;fight @EnK_)")]
		public async Task Fight(IUser user)
		{
			if (Context.User.Mention != user.Mention && SwitchCaseString == "nofight")
			{
				SwitchCaseString = "fight_p1";
				player1 = Context.User.Mention;
				player2 = user.Mention;

				string[] whoStarts = new string[]
				{
					Context.User.Mention,
					user.Mention
				};

				Random rand = new Random();

				int randIdx = rand.Next(whoStarts.Length);
				string text = whoStarts[randIdx];

				whosTurn = text;
				if (text == Context.User.Mention)
				{
					whoWaits = user.Mention;
				}
				else
				{
					whoWaits = Context.User.Mention;
				}

				await ReplyAsync("Fight started between " + Context.User.Mention + " and " + user.Mention + "!\n\n" + player1 + " you got " + health1 + " health!\n" + player2 + " you got " + health2 + " health!\n\n" + text + " your turn!");
			}
			else
			{
				await ReplyAsync(Context.User.Mention + " sorry, but there is already a fight going on, or you actually tried to commit suicide.");
			}
		}

		/// <summary>
		/// You wimp.
		/// </summary>
		/// <returns></returns>
		[Command("giveup")]
		[Alias("giveUp", "forfeit")]
		[Remarks("Stops the fight and gives up.")]
		public async Task GiveUp()
		{
			if (SwitchCaseString == "fight_p1")
			{
				await ReplyAsync("The fight stopped.");
				SwitchCaseString = "nofight";
				health1 = 100;
				health2 = 100;
			}
			else
			{
				await ReplyAsync("There is no fight to stop.");
			}
		}

		/// <summary>
		/// Hack n slash!
		/// </summary>
		/// <returns></returns>
		[Command("slash")]
		[Remarks("Slashes your foe with a sword. Good accuracy and medium damage")]
		public async Task Slash()
		{
			if (SwitchCaseString == "fight_p1")
			{
				if (whosTurn == Context.User.Mention)
				{
					Random rand = new Random();

					int randIdx = rand.Next(1, 6);
					if (randIdx != 1)
					{
						Random rand2 = new Random();

						int randIdx2 = rand2.Next(7, 15);

						if (Context.User.Mention != player1)
						{
							health1 = health1 - randIdx2;
							if (health1 > 0)
							{
								placeHolder = whosTurn;
								whosTurn = whoWaits;
								whoWaits = placeHolder;

								await ReplyAsync(Context.User.Mention + " you hit and did " + randIdx2 + " damage!\n\n" + player1 + " got " + health1 + " health leaft!\n" + player2 + " got " + health2 + " health left!\n\n" + whosTurn + ", Your turn!");
							}
							else
							{
								await ReplyAsync(Context.User.Mention + " you hit and did " + randIdx2 + " damage!\n\n" + player1 + " died. " + player2 + " won!");
								SwitchCaseString = "nofight";
								health1 = 100;
								health2 = 100;
							}
						}
						else if (Context.User.Mention == player1)
						{
							health2 = health2 - randIdx2;
							if (health2 > 0)
							{
								placeHolder = whosTurn;
								whosTurn = whoWaits;
								whoWaits = placeHolder;

								await ReplyAsync(Context.User.Mention + " you hit and did " + randIdx2 + " damage!\n\n" + player1 + " got " + health1 + " health left!\n" + player2 + " got " + health2 + " health left!\n\n" + whosTurn + ", your turn!");
							}
							else
							{
								await ReplyAsync(Context.User.Mention + " you hit and did " + randIdx2 + " damage!\n\n" + player2 + " died. " + player1 + " won!");
							}
						}
						else
						{
							placeHolder = whosTurn;
							whosTurn = whoWaits;
							whoWaits = placeHolder;

							await ReplyAsync(Context.User.Mention + " sorry, but you missed!\n" + whosTurn + ", your turn!");
						}
					}
					else
					{
						await ReplyAsync(Context.User.Mention + " it is not your turn");
					}
				}
				else
				{
					await ReplyAsync("There is no fight at the moment. Sorry!");
				}
			}
		}

		/// <summary>
		/// Spinnedy spin!
		/// </summary>
		/// <param name="_user1">The user1.</param>
		/// <param name="_user2">The user2.</param>
		/// <param name="_user3">The user3.</param>
		/// <param name="_user4">The user4.</param>
		/// <returns></returns>
		[Command("spin")]
		[Remarks("If you can't decide who to pick, use this.")]
		public async Task ArrowSpinAsync(IGuildUser _user1 = null, IGuildUser _user2 = null, IGuildUser _user3 = null, IGuildUser _user4 = null)
		{
			Random rand = new Random();
			IVoiceChannel channel2 = null;

			IGuildUser[] randomUsers = new IGuildUser[4];

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

			ImageCore.ImageCore core = new ImageCore.ImageCore();

			ImageSharp.Image<Rgba32> user1 = await core.StartStreamAsync(randomUsers[0]);
			ImageSharp.Image<Rgba32> user2 = await core.StartStreamAsync(randomUsers[1]);
			ImageSharp.Image<Rgba32> user3 = await core.StartStreamAsync(randomUsers[2]);
			ImageSharp.Image<Rgba32> user4 = await core.StartStreamAsync(randomUsers[3]);

			ImageSharp.Image<Rgba32> arrow = await core.StartStreamAsync(path: "C:/Users/LordaS/Desktop/Rica Botpaw/RicaBotpaw/images/arrow.png");

			ImageSharp.Image<Rgba32> finalImg = new ImageSharp.Image<Rgba32>(500, 500);

			Size size250 = new Size(250, 250);
			Size size500 = new Size(500, 500);

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
		}
    }
}
