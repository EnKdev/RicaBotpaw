using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Cooldown;
using RicaBotpaw.Logging;

namespace RicaBotpaw.Modules
{
	public class ServerconfModule : ModuleBase
	{
		private long publicModule;
		private long imagingModule;
		private long gameModule;
		private long nsfwFeatures;
		private long sfwFeatures;
		private long randImgFeatures;
		private long hashFeatures;
		private ulong guild;
		private CommandService _service;

		public ServerconfModule(CommandService service)
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

		[Command("conf", RunMode = RunMode.Async)]
		[Remarks("Server Configurations!")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task Conf(int publicFlag, int imagingFlag, int nsfwFlag, int sfwFlag, int randImgFlag, int hashFlag, [Remainder] IGuild g = null)
		{
			var user = Context.User;
			await CheckIfUserIsOnCooldown(user);

			if (UserCooldown.UserIsInCooldown == false)
			{
				if (g == null)
				{
					g = Context.Guild;
					var name = g.Id + "_config";

					Config.Modules mod = new Config.Modules
					{
						Comment = "This contains the modules",
						Guild = g.Id,
						ModImg = imagingFlag,
						ModPub = publicFlag,
						ModNSFW = nsfwFlag,
						ModSFW = sfwFlag,
						ModRandImg = randImgFlag,
						ModHash = hashFlag
					};

					// Config.ServerModulesConfig sMC = new ServerModulesConfig()
					// {
					//	   Comment = "This rconf file will store values for individual guilds. 1 = enabled, 0 = disabled",
					//	   Modules = mod
					// };

					// if (File.Exists($"./serv_configs/{name}.rconf"))
					// {
					//	   File.Delete($"./serv_configs/{name}.rconf");
					// }

					using (StreamWriter file = File.CreateText($"./Data/serv_configs/{name}.rconf"))
					{
						var fileText = JsonConvert.SerializeObject(mod);
						await file.WriteAsync(fileText);
						await ReplyAsync($"Serverconfig file {name}.rconf created at /serv_configs");
					}

					await UserCooldown.PutInCooldown(user);
				}
			}
		}

		[Command("confhelp", RunMode = RunMode.Async)]
		[Remarks("This is all new, you better read it!")]
		public async Task ConfHelp()
		{
			await ReplyAsync(ModStrings.ConfigHelp);
		}

		[Command("modulecheck", RunMode = RunMode.Async)]
		[Remarks("This checks the modules for a guild")]
		public async Task ModCheck([Remainder] IGuild g = null)
		{
			var user = Context.User;
			await CheckIfUserIsOnCooldown(user);

			if (UserCooldown.UserIsInCooldown == false)
			{
				if (g == null)
				{
					g = Context.Guild;
					var name = g.Id + "_config";

					if (!File.Exists($"./Data/serv_configs/{name}.rconf"))
					{
						await ReplyAsync(ModStrings.GuildNoConfigFile);
						return;
					}
					
					var fileText = File.ReadAllText($"./Data/serv_configs/{name}.rconf");
					var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText);

					publicModule = mods.ModPub;
					imagingModule = mods.ModImg;
					nsfwFeatures = mods.ModNSFW;
					sfwFeatures = mods.ModSFW;
					randImgFeatures = mods.ModRandImg;
					hashFeatures = mods.ModHash;
					

					EmbedBuilder embed = new EmbedBuilder
					{
						Color = new Color(15, 158, 120),
						ThumbnailUrl = $"{g.IconUrl}",
						Title = $"Modules enabled for Guild {g.Name} (Id: {g.Id})",
						Description = "Short notice: 0 = disabled, 1 = enabled.\n" +
						              $"Public Commands (PublicModule): {publicModule}\n" +
						              $"Imaging Commands (ImagingModule): {imagingModule}\n" +
						              $"Game Commands (GamesModule): {gameModule}\n" +
									  $"NSFW Image Searching (NSFWFeatures): {nsfwFeatures}\n" +
									  $"SFW Image Searching (SFWFeatures): {sfwFeatures}\n" +
									  $"Random Images (RandImgFeatures): {randImgFeatures}\n" +
									  $"Hashing Features (HashFeatures): {hashFeatures}"
					};

					await ReplyAsync("", false, embed);
				}

				await UserCooldown.PutInCooldown(user);
			}
		}
	}
}