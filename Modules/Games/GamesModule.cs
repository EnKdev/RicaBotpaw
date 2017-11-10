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
