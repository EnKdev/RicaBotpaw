using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RicaBotpaw.Config;
using RicaBotpaw.Logging;

namespace RicaBotpaw.Modules
{
	public class ServerconfModule : ModuleBase
	{
		private long publicModule;
		private long ecoFeatures;
		private long gamblingFeatures;
		private long pollFeatures;
		private long imagingModule;
		private long gameModule;
		private long nsfwFeatures;
		private long sfwFeatures;
		private long randImgFeatures;
		private ulong guild;

		private CommandService _service;

		public ServerconfModule(CommandService service)
		{
			_service = service;
		}

		[Command("conf", RunMode = RunMode.Async)]
		[Remarks("Server Configurations!")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task Conf(int publicFlag, int economyFlag, int gamblingFlag, int pollFlag, int imagingFlag, int gameFlag, int nsfwFlag, int sfwFlag, int randImgFlag, [Remainder] IGuild g = null)
		{
			if (BotCooldown.isCooldownRunning == false)
			{
				if (g == null)
				{
					g = Context.Guild;
					var name = g.Id + "_config";

					Config.Modules mod = new Config.Modules
					{
						Comment = "This contains the modules",
						Guild = g.Id,
						ModGame = gameFlag,
						ModImg = imagingFlag,
						ModPub = publicFlag,
						ModPubEco = economyFlag,
						ModPubEcoGmb = gamblingFlag,
						ModPubPoll = pollFlag,
						ModNSFW = nsfwFlag,
						ModSFW = sfwFlag,
						ModRandImg = randImgFlag
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

					using (StreamWriter file = File.CreateText($"./serv_configs/{name}.rconf"))
					{
						var fileText = JsonConvert.SerializeObject(mod);
						await file.WriteAsync(fileText);
						await ReplyAsync($"Config file {name}.rconf created at /serv_configs");
					}

					await BotCooldown.Cooldown();
				}
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
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
			if (BotCooldown.isCooldownRunning == false)
			{
				if (g == null)
				{
					g = Context.Guild;
					var name = g.Id + "_config";

					if (!File.Exists($"./serv_configs/{name}.rconf"))
					{
						await ReplyAsync(ModStrings.GuildNoConfigFile);
						return;
					}
					
					var fileText = File.ReadAllText($"./serv_configs/{name}.rconf");
					var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText);

					publicModule = mods.ModPub;
					ecoFeatures = mods.ModPubEco;
					gamblingFeatures = mods.ModPubEcoGmb;
					pollFeatures = mods.ModPubPoll;
					imagingModule = mods.ModImg;
					gameModule = mods.ModGame;
					nsfwFeatures = mods.ModNSFW;
					sfwFeatures = mods.ModSFW;
					randImgFeatures = mods.ModRandImg;
					

					EmbedBuilder embed = new EmbedBuilder
					{
						Color = new Color(15, 158, 120),
						ThumbnailUrl = $"{g.IconUrl}",
						Title = $"Modules enabled for Guild {g.Name} (Id: {g.Id})",
						Description = "Short notice: 0 = disabled, 1 = enabled.\n" +
						              $"Public Commands (PublicModule): {publicModule}\n" +
						              $"Economy (EcoFeatures): {ecoFeatures}\n" +
						              $"Gambling (GamblingFeatures): {gamblingFeatures}\n" +
						              $"Polls (PollFeature): {pollFeatures}\n" +
						              $"Imaging Commands (ImagingModule): {imagingModule}\n" +
						              $"Game Commands (GamesModule): {gameModule}\n" +
									  $"NSFW Image Searching (NSFWFeatures): {nsfwFeatures}\n" +
									  $"SFW Image Searching (SFWFeatures): {sfwFeatures}\n" +
									  $"Random Images (RandImgFeatures): {randImgFeatures}"
					};

					await ReplyAsync("", false, embed);
				}

				await BotCooldown.Cooldown();
			}
			else
			{
				await ReplyAsync(BotCooldown.cooldownMsg);
			}
		}
	}
}