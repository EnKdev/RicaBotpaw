// P_Eco_GamblingSub.cs
// The Gambling subclass of the economy subclass located in PublicModule.cs
// This is to split the public module into files so that the subclasses have their own file.


using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Logging;
using System;
using System.Linq;
using System.Threading;
using Discord.WebSocket;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules
{
	public class P_Eco_GamblingSub : ModuleBase
	{
		private int featEnable;

		private CommandService _service;

		public P_Eco_GamblingSub(CommandService service)
		{
			_service = service;
		}

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
							if (mods.ModPubEcoGmb == 1) // Check for the Feature to be enabled,
							{
								featEnable = 1;
							}
							else // If it is not 1, but 2 or higher than 1 or actually 0, then the module is disabled by default
							{
								featEnable = 0;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Either you win or you lose.
		/// </summary>
		/// <param name="bet">The bet.</param>
		/// <returns></returns>
		[Command("bet", RunMode = RunMode.Async)]
		[Remarks("Place your bets and roll the dice!")]
		public async Task betCmd(int bet)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
				else
				{
					await ReplyAsync(BotCooldown.cooldownMsg);
				}
			}
			else
			{
				await ReplyAsync(ModStrings.GamblingNotEnabled);
			}
		}
	}
}