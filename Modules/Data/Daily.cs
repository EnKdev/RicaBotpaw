// Daily.cs
// This feels like this is the only interactive file inside this folder actually
// It contains a weird, yet... somehow working implementation of a daily method.

using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Attributes;
using RicaBotpaw.Libs;
using Measure = RicaBotpaw.Attributes.Measure;

namespace RicaBotpaw.Modules.Data
{
	public class Daily : ModuleBase
	{
		private CommandService _service;
		private DateTime _dailyTime;
		private int userExists;
		private int hasDailyPassed;

		public Daily(CommandService service)
		{
			service = _service;
		}

		private async Task CheckExistingJsonUser([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;

			if (!File.Exists($"./Data/users/{u.Id.ToString()}_{u.Username.ToString()}.rbuser"))
			{
				await ReplyAsync("User does not exist");
				userExists = 0;
				return;
			}
			else
			{
				await ReplyAsync($"Found user {u.Username.ToString()} in Json Database, reading file...");
				userExists = 1;
			}
		}

		private async Task CheckDaily([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;

			if (!File.Exists($"./Data/users/{u.Id.ToString()}_{u.Username.ToString()}.rbuser"))
			{
				await ReplyAsync("User does not exist");
				return;
			}
			else
			{
				var fileName = $"{u.Id}_{u.Username}";
				var fileText = File.ReadAllText($"./Data/users/{fileName}.rbuser");
				var fileText1 = EncoderUtils.B64Decode(fileText);
				var data = JsonConvert.DeserializeObject<DiscordUserJSON>(fileText1);
				var now = DateTime.Now;
				var daily = data.Daily;

				if (now.Date > daily.Date)
				{
					Console.WriteLine("Daily passed, new day has started since the last time the command has been triggered.");
					hasDailyPassed = 1;
				}
				else
				{
					Console.WriteLine("The command was triggered on the same day by the same user, a new day hasn't been started yet or the user has no daily yet.");
					await ReplyAsync(
						"The command was triggered on the same day by the same user, a new day hasn't been started yet or the user has no daily yet.");
					hasDailyPassed = 0;
				}
			}
		}

		[Command("daily", RunMode = RunMode.Async), RBRatelimit(1, 5, Measure.Seconds)]
		[Remarks("Your daily payout man! Works whenever a new day has passed for the bot for each user.")]
		public async Task DailyTask([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;
			var u1 = Context.Message.Author;
			await CheckExistingJsonUser(u);
			await CheckDaily(u);

			if (userExists == 1)
			{
				if (hasDailyPassed == 1)
				{
					var fileName = $"{u.Id}_{u.Username}";
					var fileText = File.ReadAllText($"./Data/users/{fileName}.rbuser");
					var fileText1 = EncoderUtils.B64Decode(fileText);
					var data = JsonConvert.DeserializeObject<DiscordUserJSON>(fileText1);
					var mons = 200;
					var guidToKeep = data.UserGuid;
					var userID = data.UserId;
					var userName = data.Username;
					var tokensToKeep = data.Tokens;
					var oldMoneyValue = data.Money;
					var newMoneyValue = oldMoneyValue + mons;
					DateTime now = DateTime.Now;

					DiscordUserJSON data2 = new DiscordUserJSON
					{
						Daily = now,
						Money = newMoneyValue,
						Tokens = tokensToKeep,
						UserGuid = guidToKeep,
						UserId = userID,
						Username = userName
					};

					using (StreamWriter file = File.CreateText($"./Data/users/{fileName}.rbUser"))
					{
						var fileText2 = JsonConvert.SerializeObject(data2);
						var fileText3 = EncoderUtils.B64Encode(fileText2);
						await file.WriteAsync(fileText3);
						await Context.Channel.SendMessageAsync(
							$"{u.Mention}, you got your daily {mons}$!\nYour new balance is now {newMoneyValue}!");
					}
				}
			}
		}
	}
}