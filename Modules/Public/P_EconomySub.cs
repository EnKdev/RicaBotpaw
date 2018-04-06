// P_EconomySub.cs
// The economy subclass of PublicModule.cs
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
using System;

namespace RicaBotpaw.Modules
{
	public class Economy : ModuleBase
	{
		private int featEnable;
		private int gNoticeSent;

		private CommandService _service;

		public Economy(CommandService service)
		{
			_service = service;
		}

		private async Task CheckEnableFeatureModule([Remainder] IGuild g = null)
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
			if (mods.ModPubEco == 1)
			{
				featEnable = 1;
				return;
			}
			featEnable = 0;
		}

		/// <summary>
		/// First you gotta open a bank account before you get any money.
		/// </summary>
		/// <returns></returns>
		[Command("openbank", RunMode = RunMode.Async)]
		[Remarks("Opens your bank account! Woah!")]
		public async Task bankC()
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.EconomyNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.EconomyNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.EconomyNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		

		/// <summary>
		/// the payment process
		/// </summary>
		/// <param name="payUser">The pay user.</param>
		/// <param name="recieveUser">The recieve user.</param>
		/// <param name="money">The money.</param>
		/// <returns></returns>
		[Command("pay", RunMode = RunMode.Async)]
		[Remarks("Pays the user with stored money")]
		public async Task PayMoney(IUser recieveUser, int money, [Remainder] IUser payUser = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (payUser == null)
					{
						payUser = Context.User;
						Database.PayMoney1(payUser, money);
						Database.PayMoney2(recieveUser, money);
						await ReplyAsync($"Successfully paid {recieveUser} {money} Dollars!");
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
					await ReplyAsync(ModStrings.EconomyNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.EconomyNotEnabled);
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