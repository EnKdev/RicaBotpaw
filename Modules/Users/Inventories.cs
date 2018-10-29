using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Cooldown;
using RicaBotpaw.Libs;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules.Users
{
	public class Inventories : ModuleBase
	{
		private CommandService _service;
		private int _invExists;

		public Inventories(CommandService service)
		{
			_service = service;
		}

		private async Task CheckIfUserIsOnCooldown([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;

			if (UserCooldown.UsersInCooldown.Contains(u))
			{
				UserCooldown.UserIsInCooldown = true;
				ReplyAsync("You're in cooldown! Please wait 5 seconds!");
			}
			else
			{
				UserCooldown.UserIsInCooldown = false;
			}
		}

		private async Task CheckExistingJsonInventory([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;

			if (!File.Exists($"./Data/users/inventories/{u.Id.ToString()}_Inventory.rbinv"))
			{
				await ReplyAsync("Inventory does not exist");
				_invExists = 0;
				return;
			}
			else
			{
				await ReplyAsync($"Found user's Inventory in Json Database, reading file...");
				_invExists = 1;
			}
		}

		// Don't uncomment this when you ensured the functionality in further tests! #SecretFeature
		[Command("invCreate", RunMode = RunMode.Async)]
		[Remarks("Creates your own user inventory!")]
		public async Task CreateInventory()
		{
			var u = Context.Message.Author;
			await CheckIfUserIsOnCooldown(u);
			{
				if (UserCooldown.UserIsInCooldown == false)
				{
					var fileName = u.Id.ToString() + "_Inventory";

					UserInventories invData = new UserInventories
					{
						Inventory = new Inventory
						{
							Badges = new[]
							{
							""
						},
							Consumables = new[]
							{
							""
						},
							Titles = new[]
							{
							""
						}
						},
						UserId = u.Id,
						Username = u.Username
					};

					using (StreamWriter writer = File.CreateText($"./Data/users/inventories/{fileName}.rbinv"))
					{
						var fileText = JsonConvert.SerializeObject(invData);
						var fileText1 = EncoderUtils.B64Encode(fileText);
						await writer.WriteAsync(fileText1);
						await ReplyAsync("Created inventory of User " + u.Username + " at /Data/users/inventories");
					}

					UserCooldown.PutInCooldown(u);
				}
			}
		}

		[Command("invRead", RunMode = RunMode.Async)]
		[Remarks("Let's you view your inventory!")]
		public async Task ReadInventory()
		{
			List<string> badgeList = new List<string>();
			List<string> conList = new List<string>();
			List<string> tList = new List<string>();

			var u = Context.Message.Author;
			await CheckIfUserIsOnCooldown(u);
			await CheckExistingJsonInventory(u);
			{
				if (UserCooldown.UserIsInCooldown == false)
				{
					if (_invExists == 1)
					{
						var fileName = u.Id.ToString() + "_Inventory";
						var fileText = File.ReadAllText($"./Data/users/inventories/{fileName}.rbinv");
						var fileText1 = EncoderUtils.B64Decode(fileText);
						var invData = JsonConvert.DeserializeObject<UserInventories>(fileText1);
						var badges = invData.Inventory.Badges;
						var bdg = badgeList;
						var consum = invData.Inventory.Consumables;
						var c = conList;
						var titles = invData.Inventory.Titles;
						var t1 = tList;

						foreach (var badge in badges)
						{
							string bdge = badge;
							bdge = bdge.Replace("bdg_", "");
							badgeList.Add(bdge);
						}

						foreach (var con in consum)
						{
							string c1 = con;
							c1 = c1.Replace("csm_", "");
							conList.Add(c1);
						}

						foreach (var t in titles)
						{
							string title = t;
							title = title.Replace("t_", "");
							tList.Add(title);
						}

						var embed = new EmbedBuilder
						{
							Color = new Color(69, 221, 192),
							Title = $"{u.Username}'s Inventory",
							Description = $"**Badges**:\n{string.Join(", ", bdg)}\n" +
										  $"**Consumables**:\n{string.Join(", ", c)}\n" +
										  $"**Titles**:\n{string.Join(", ", t1)}"
						};
						embed.WithFooter(new EmbedFooterBuilder().WithText($"Inventory fetched at {DateTime.Now}"));
						await Context.Channel.SendMessageAsync("", false, embed);
						UserCooldown.PutInCooldown(u);
					}
					else
					{
						await ReplyAsync("Inventory not found. Are you sure you created yours?");
						UserCooldown.PutInCooldown(u);
					}
				}
			}
		}
	}
}