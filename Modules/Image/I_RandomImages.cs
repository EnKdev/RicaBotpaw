using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using RicaBotpaw;
using RicaBotpaw.Logging;
using Discord;
using System.IO;
using Newtonsoft.Json;

namespace RicaBotpaw.Modules.Image
{
	public class RandomImages : ModuleBase
	{
		private CommandService _service;

		public RandomImages(CommandService service)
		{
			_service = service;
		}

		private int featEnable;
		private int gNoticeSent;

		private async Task CheckRandomImageFeatureEnabled([Remainder] IGuild g = null)
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
					"Specified Guild ID doesn't match saved Guild ID in config file."); // This should actually never happen unless you specified another guild... however that would work.
				return;
			}
			if (mods.ModRandImg == 1)
			{
				featEnable = 1;
				return;
			}
			featEnable = 0;
		}

		/// <summary>
		/// Random animals!
		/// </summary>
		/// <returns></returns>
		[Command("random", RunMode = RunMode.Async)]
		[Remarks("Sends you a random image based on the species you entered. Powered by FurBot")]
		public async Task RandomImage([Remainder] string species = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckRandomImageFeatureEnabled(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					switch (species)
					{
						case "fox":
							Console.WriteLine("Making FurBot Server Call...");
							using (var client = new HttpClient(new HttpClientHandler
							{
								AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
							}))
							{
								string websiteUrl = "http://fur.im/fox";
								client.BaseAddress = new Uri(websiteUrl);

								HttpResponseMessage res = client.GetAsync("").Result;
								res.EnsureSuccessStatusCode();

								string result = await res.Content.ReadAsStringAsync();
								var json = JObject.Parse(result);

								string FoxImage = json["file"].ToString();

								await ReplyAsync(FoxImage);
								await BotCooldown.Cooldown();
							}
							break;
						case "wolf":
							Console.WriteLine("Making FurBot Server Call...");
							using (var client = new HttpClient(new HttpClientHandler
							{
								AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
							}))
							{
								string websiteUrl = "http://fur.im/wolf";
								client.BaseAddress = new Uri(websiteUrl);

								HttpResponseMessage res = client.GetAsync("").Result;
								res.EnsureSuccessStatusCode();

								string result = await res.Content.ReadAsStringAsync();
								var json = JObject.Parse(result);

								string WolfImage = json["file"].ToString();

								await ReplyAsync(WolfImage);
								await BotCooldown.Cooldown();
							}
							break;
						case "axolotl":
							Console.WriteLine("Making FurBot Server Call...");
							using (var client = new HttpClient(new HttpClientHandler
							{
								AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
							}))
							{
								string websiteUrl = "http://fur.im/axo";
								client.BaseAddress = new Uri(websiteUrl);

								HttpResponseMessage res = client.GetAsync("").Result;
								res.EnsureSuccessStatusCode();

								string result = await res.Content.ReadAsStringAsync();
								var json = JObject.Parse(result);

								string AxoImage = json["file"].ToString();

								await ReplyAsync(AxoImage);
								await BotCooldown.Cooldown();
							}
							break;
						case "snake":
							Console.WriteLine("Making FurBot Server Call...");
							using (var client = new HttpClient(new HttpClientHandler
							{
								AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
							}))
							{
								string websiteUrl = "http://fur.im/snek";
								client.BaseAddress = new Uri(websiteUrl);

								HttpResponseMessage res = client.GetAsync("").Result;
								res.EnsureSuccessStatusCode();

								string result = await res.Content.ReadAsStringAsync();
								var json = JObject.Parse(result);

								string SnakeImage = json["file"].ToString();

								await ReplyAsync(SnakeImage);
								await BotCooldown.Cooldown();
							}
							break;
						// case "bunny":
						//		Console.WriteLine("Making FurBot Server Call...");
						//		using (var client = new HttpClient(new HttpClientHandler
						//		{
						//			AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
						//		}))
						//		{
						//			string websiteUrl = "http://fur.im/bun";
						//			client.BaseAddress = new Uri(websiteUrl);

						//			HttpResponseMessage res = client.GetAsync("").Result;
						//			res.EnsureSuccessStatusCode();

						//			string result = await res.Content.ReadAsStringAsync();
						//			var json = JObject.Parse(result);

						//			string BunnyImage = json["file"].ToString();

						//			await ReplyAsync(BunnyImage);
						//			await BotCooldown.Cooldown();
						//		}
						//		break;
						default:
							await ReplyAsync("Invalid input detected. The only species supported are:\n`fox, wolf, axolotl, snake`");
							return;

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
					await ReplyAsync(ModStrings.PublicNotEnabled);
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
