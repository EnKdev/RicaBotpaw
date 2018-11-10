// I_RandomImages.cs
// The random image subclass of ImageModule.cs
// This is to split the image module into files so that the subclasses have their own file.

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using RicaBotpaw.Logging;
using Discord;
using System.IO;
using Newtonsoft.Json;
using RicaBotpaw.Attributes;
using RicaBotpaw.Libs;

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

			if (!File.Exists($"./Data/serv_configs/{g.Id.ToString()}_config.rconf"))
			{
				await ReplyAsync(ModStrings.GuildNoConfigFile);
				gNoticeSent = 1;
				return;
			}

			var fileText = File.ReadAllText($"./Data/serv_configs/{g.Id.ToString()}_config.rconf");
			var fileText1 = EncoderUtils.B64Decode(fileText);
			var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText1);

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
		[Command("random", RunMode = RunMode.Async), RBRatelimit(1, 5, Measure.Seconds)]
		[Remarks("Sends you a random image based on the species you entered. Powered by FurBot")]
		public async Task RandomImage([Remainder] string species = null)
		{
			var g = Context.Guild as SocketGuild;
			var user = Context.Message.Author;
			await CheckRandomImageFeatureEnabled(g);
			
			if (featEnable == 1)
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
						case "cat":
							Console.Write("Fetching random cat image...");
							using (var client = new HttpClient(new HttpClientHandler
							{
								AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
							}))
							{
								string websiteUrl = "http://aws.random.cat/meow";
								client.BaseAddress = new Uri(websiteUrl);

								HttpResponseMessage res = client.GetAsync("").Result;
								res.EnsureSuccessStatusCode();

								string result = await res.Content.ReadAsStringAsync();
								var json = JObject.Parse(result);

								string CatImage = json["file"].ToString();

								await ReplyAsync(CatImage);
							}
							break;
						default:
							await ReplyAsync("Invalid input detected. The only species supported are:\n`fox, wolf, axolotl, snake, cat`");
							return;

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
