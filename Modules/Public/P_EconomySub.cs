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
	public class P_EconomySub : ModuleBase
	{
		private int featEnable;

		private CommandService _service;

		public P_EconomySub(CommandService service)
		{
			_service = service;
		}

		private async Task CheckEnableFeatureModule([Remainder] IGuild g = null)
		{ 
			if (g == null)
			{
				g = Context.Guild;

				if (File.Exists($"./serv_configs/{g.Id}.rconf"))
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
							if (mods.ModPubEco == 1) // Check for the Feature to be enabled
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
				await ReplyAsync(ModStrings.EconomyNotEnabled);
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
				await ReplyAsync(ModStrings.EconomyNotEnabled);
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
				await ReplyAsync(ModStrings.EconomyNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
			else
			{
				await ReplyAsync(ModStrings.EconomyNotEnabled);
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
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
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
				await ReplyAsync(ModStrings.EconomyNotEnabled);
			}
		}
	}
}