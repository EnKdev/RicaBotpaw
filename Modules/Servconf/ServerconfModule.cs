// ServerconfModule.cs
// This Module is for creating the guild configuration files!
// It also contains a method to check which modules and features are enabled.

using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Attributes;
using RicaBotpaw.Libs;
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
		private Random random;

		public ServerconfModule(CommandService service)
		{
			_service = service;
		}

		[Command("conf", RunMode = RunMode.Async), RBRatelimit(1, 5, Measure.Seconds)]
		[Remarks("Server Configurations!")]
		[RequireUserPermission(GuildPermission.Administrator)]
		public async Task Conf(int publicFlag, int imagingFlag, int nsfwFlag, int sfwFlag, int randImgFlag,
			[Remainder] IGuild g = null)
		{
			var user = Context.Message.Author;

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
					var fileText1 = EncoderUtils.B64Encode(fileText);
					await file.WriteAsync(fileText1);
					await ReplyAsync($"Serverconfig file {name}.rconf created at Data/serv_configs");
				}
			}
		}

		[Command("confhelp", RunMode = RunMode.Async)]
		[Remarks("This is all new, you better read it!")]
		public async Task ConfHelp()
		{
			await ReplyAsync(ModStrings.ConfigHelp);
		}

		[Command("modulecheck", RunMode = RunMode.Async), RBRatelimit(1, 5, Measure.Seconds)]
		[Remarks("This checks the modules for a guild")]
		public async Task ModCheck([Remainder] IGuild g = null)
		{
			var user = Context.Message.Author;

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
				var fileText1 = EncoderUtils.B64Decode(fileText);
				var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText1);

				publicModule = mods.ModPub;
				imagingModule = mods.ModImg;
				nsfwFeatures = mods.ModNSFW;
				sfwFeatures = mods.ModSFW;
				randImgFeatures = mods.ModRandImg;


				EmbedBuilder embed = new EmbedBuilder
				{
					Color = new Color(
						Convert.ToInt32(MathHelper.GetRandomIntegerInRange(random, 1, 255)),
								Convert.ToInt32(MathHelper.GetRandomIntegerInRange(random, 1, 255)),
								Convert.ToInt32(MathHelper.GetRandomIntegerInRange(random, 1, 255))
					),
					ThumbnailUrl = $"{g.IconUrl}",
					Title = $"Modules enabled for Guild {g.Name} (Id: {g.Id})",
					Description = "Short notice: 0 = disabled, 1 = enabled.\n" +
					              $"Public Commands (PublicModule): {publicModule}\n" +
					              $"Imaging Commands (ImagingModule): {imagingModule}\n" +
					              $"NSFW Image Searching (NSFWFeatures): {nsfwFeatures}\n" +
					              $"SFW Image Searching (SFWFeatures): {sfwFeatures}\n" +
					              $"Random Images (RandImgFeatures): {randImgFeatures}"
				};

				await ReplyAsync("", false, embed);
			}
		}
	}
}