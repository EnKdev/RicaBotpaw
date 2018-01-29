using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules.Sona
{
	/// <summary>
	///     The SonaModule class
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	[Remarks("For the furries out there using this bot!")]
	public class Sona : ModuleBase
	{

		/// <summary>
		///     The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		///     Initializes the sona module into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Sona(CommandService service)
		{
			_service = service;
		}


		/// <summary>
		///     Register your sona!
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="name">The name.</param>
		/// <param name="age">The age.</param>
		/// <param name="spec">The spec.</param>
		/// <param name="gend">The gend.</param>
		/// <param name="sex">The sex.</param>
		/// <returns></returns>
		[Command("register", RunMode = RunMode.Async)]
		[Remarks("Registers your sona into the sona database")]
		public async Task RegisterSona(string name, int age, string spec, string gend, string sex,
			[Remainder] IUser user = null)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (user == null)
				{
					user = Context.User;
					Database.EnterUser(user);
					Database.RegisterSona(user, name, age, spec, gend, sex);
					await ReplyAsync("Registered! :thumbsup::skin-tone-1:");
					await BotCooldown.Cooldown();
				}
				else if (user != Context.User)
				{
					await ReplyAsync("You only can register your own sona.");
					await BotCooldown.Cooldown();
				}
				else // If there is no user specified
				{
					Database.RegisterSona(user, name, age, spec, gend, sex);
					await ReplyAsync("Registered! :thumbsup::skin-tone-1:");
					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}

		/// <summary>
		///     Because fursonas are lovely beings~
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("sona", RunMode = RunMode.Async)]
		[Remarks("Returns info about sona by a certain user")]
		public async Task SonaInfo([Remainder] IUser user = null)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (user == null)
				{
					user = Context.User;
					var embed = new EmbedBuilder
					{
						Color = new Color(109, 218, 37)
					};

					if (user == null)
						user = Context.User;

					var result = Database.GetSona(user);

					if (result.Count == null)
					{
						await ReplyAsync("You haven't registered a sona dear.");
						await ReplyAsync(BotCooldown.cooldownMsg);
					}
					else
					{
						embed.Description = user.Username + "'s Sona is as followed:\n\n"
											+ ":arrow_right:" + "Sona: " + result.FirstOrDefault().SonaName + "\n"
											+ ":arrow_right:" + "Age: " + result.FirstOrDefault().Age + "\n"
											+ ":arrow_right:" + "Species: " + result.FirstOrDefault().Species + "\n"
											+ ":arrow_right:" + "Gender: " + result.FirstOrDefault().Gender + "\n"
											+ ":arrow_right:" + "Sexuality: " + result.FirstOrDefault().Sexuality;

						await Context.Channel.SendMessageAsync("", false, embed);
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
}