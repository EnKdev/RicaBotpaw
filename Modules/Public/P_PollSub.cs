// P_PollSub.cs
// The poll subclass of PublicModule.cs
// This is to split the public module into files so that the subclasses have their own file.


using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using RicaBotpaw.Logging;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules
{
	public class P_PollSub : ModuleBase
	{

		private int featEnable;

		private CommandService _service;

		public P_PollSub(CommandService service)
		{
			_service = service;
		}

		/// <summary>
		/// This is the string which helps us checking if a poll is currently running. If it says yes, then there can't be another poll made until it says no again
		/// </summary>
		private static string _isAPollRunning = "no";

		/// <summary>
		/// Security-Measure for preventing that other users end your poll
		/// </summary>
		private static string _userMadePoll = "";

		private async Task CheckEnableFeatureModule([Remainder] IGuild g = null)
		{
			if (g == null)
			{
				g = Context.Guild;

				if (!File.Exists($"./serv_configs/{g.Id}.rconf"))
				{
					await ReplyAsync(ModStrings.GuildNoConfigFile);
				}
				else
				{
					using (StreamReader file = File.OpenText($"./serv_configs/{g.Id}.rconf"))
					{
						JsonSerializer ser = new JsonSerializer();
						Config.Modules mods = (Config.Modules)ser.Deserialize(file, typeof(Config.Modules));

						if (mods.Guild != g.Id)
						{
							await ReplyAsync("Specified Guild ID doesn't match saved Guild ID in config file."); // This should actually never happen
						}
						else
						{
							if (mods.ModPubPoll == 1) // Check for the Feature to be enabled
							{
								featEnable = 1;
							}
							else // If it is not 1, but 2 or higher than 1 or even 0, then the module is disabled by default
							{
								featEnable = 0;
							}
						}
					}
				}
			}
		}

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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PollNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PollNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.PollNotEnabled);
			}
		}
	}
}