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
		private CommandService _service;

		public ServerconfModule(CommandService service)
		{
			_service = service;
		}

		[Command("conf", RunMode = RunMode.Async)]
		[Remarks("Server Configurations!")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task Conf(int publicFlag, int economyFlag, int gamblingFlag, int pollFlag, int imagingFlag, int gameFlag, [Remainder] IGuild g = null)
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
						ModPubPoll = pollFlag
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
	}
}