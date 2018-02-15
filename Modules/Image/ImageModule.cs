using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ImageSharp;
using ImageSharp.Dithering;
using ImageSharp.Processing;
using Newtonsoft.Json;
using RicaBotpaw.Logging;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules.Image
{
	/// <summary>
	///     This is the image module class
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	[Remarks("Everything related to images and such work with this module. The bot's specially designed Imaging Library takes care of the rest.")]
	public class Imaging : ModuleBase
	{
		private int modEnable;
		private int gNoticeSent;

		/// <summary>
		///     The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		///     This initializes the ImageModule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Imaging(CommandService service)
		{
			_service = service;
		}

		private async Task CheckEnabledImageModule([Remainder] IGuild g = null)
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
			if (mods.ModImg == 1)
			{
				modEnable = 1;
				return;
			}
			modEnable = 0;
		}

		/// <summary>
		/// Flippedy flip!
		/// </summary>
		/// <param name="degrees">The degrees.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		[Command("flip", RunMode = RunMode.Async)]
		[Remarks("Flips your Discord Avatar or another image specified through an URL")]
		public async Task FlipImage(int degrees = 888, string url = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledImageModule(g);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					await flipImage(degrees, url);
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
					await ReplyAsync(ModStrings.ImagingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		/// <summary>
		/// The task used inside FlipImage
		/// </summary>
		/// <param name="degrees">The degrees.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public async Task flipImage(int degrees, string url) // We don't need to call the cooldown here as it is no command but just the framework behind the flipping command
		{

			if (degrees <= 0)
			{
				await ReplyAsync("Please specify a value greater than 0");
			}
			else if (degrees == 888)
			{
				await ReplyAsync("Pleace specify a number");
			}
			else if (degrees >= 360)
			{
				await ReplyAsync("Please specify a number smaller than 360 degrees");
			}
			else if (url == null)
			{
				var core = new ImageCore.ImageCore();
				var img = await core.StartStreamAsync(Context.User);
				img.Rotate(degrees);
				await core.StopStreamAsync(Context.Message, img);
			}
			else
			{
				var core = new ImageCore.ImageCore();
				var img = await core.StartStreamAsync(Context.User, url);
				img.Rotate(degrees);
				await core.StopStreamAsync(Context.Message, img);
			}
		}


		/// <summary>
		///     Instagram Emulator 2017
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("filter", RunMode = RunMode.Async)]
		[Remarks("We all know Instagram to be honest")]
		public async Task filterImage(string filter = null, [Remainder] SocketUser user = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnabledImageModule(g);

			if (modEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					var task = Task.Run(async () =>
					{
						var core = new ImageCore.ImageCore();
						Image<Rgba32> img = null;

						if (filter != "help")
						{
							if (user != null) img = await core.StartStreamAsync(user);
							else img = await core.StartStreamAsync(Context.User);
							img.Resize(500, 500);
						}

						var rand = new Random();
						string[] randomFilters =
						{
						"sepia",
						"vignette",
						"polaroid",
						"pixelate",
						"oilpaint",
						"lomograph",
						"kodachrome",
						"invert",
						"grayscale",
						"glow",
						"sharpen",
						"blur",
						"dither",
						"detectedges",
						"colorblind",
						"blackwhite",
						"threshold"
						};

						if (filter == null || filter == "random") filter = randomFilters[rand.Next(0, randomFilters.Length)];
						await Context.Channel.SendMessageAsync($"Applying filter = {filter}");
						switch (filter)
						{
							case "sepia":
								img.Sepia();
								break;
							case "vignette":
								img.Vignette();
								break;
							case "polaroid":
								img.Polaroid();
								break;
							case "pixelate":
								img.Pixelate(10);
								break;
							case "oilpaint":
								img.OilPaint();
								break;
							case "lomograph":
								img.Lomograph();
								break;
							case "kodachrome":
								img.Kodachrome();
								break;
							case "invert":
								img.Invert();
								break;
							case "glow":
								img.Glow();
								break;
							case "sharpen":
								img.GaussianSharpen();
								break;
							case "blur":
								img.GaussianBlur();
								break;
							case "dither":
								img.Dither(new JarvisJudiceNinke(), .5f);
								break;
							case "detectedges":
								img.DetectEdges();
								break;
							case "colorblind":
								img.ColorBlindness(ColorBlindness.Achromatomaly);
								break;
							case "blackwhite":
								img.BlackWhite();
								break;
							case "threshold":
								img.BinaryThreshold(.5f);
								break;

							case "help":
								var embed = new EmbedBuilder();
								embed.AddInlineField("Filter help:",
									"You can try any of these filters/operations to yours or another users avatar:\n" +
									"```sepia, vignette, polaroid, pixelate, oilpaint, lomograph, kodachrome, invert, grayscale, glow, sharpen, blur, dither, detectedges, colorblind, blackwhite, threshold or 'random' for a random selection.```");
								await Context.Channel.SendMessageAsync("", false, embed);
								return;
						}
						await core.StopStreamAsync(Context.Message, img);
						await BotCooldown.Cooldown();
					});
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
					await ReplyAsync(ModStrings.ImagingNotEnabled);
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