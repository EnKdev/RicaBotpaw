using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace RicaBotpaw.Modules.Games
{
	public class GamesModule : ModuleBase
	{
		// 8ball

		string[] eightBallPredicts = new string[]
		{
			"It is very unlikely.",
			"I don't think so...",
			"Yes!",
			"I don't know",
			"No."
		};

		Random rand = new Random();

		private CommandService _service;

		public GamesModule(CommandService service)
		{
			_service = service;
		}

		[Command("8ball")]
		[Remarks("Gives a prediction")]
		public async Task EightBall([Remainder] string input)
		{
			int randomIndex = rand.Next(eightBallPredicts.Length);
			string text = eightBallPredicts[randomIndex];
			await ReplyAsync(Context.User.Mention + ", " + text);
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
    }
}
