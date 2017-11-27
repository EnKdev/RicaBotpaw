using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using System.Net.Http;
using ImageSharp;
using RicaBotpaw.ImageCore;
using ImageSharp.Dithering;
using ImageSharp.Processing;

namespace RicaBotpaw.Modules.Image
{
	/// <summary>
	/// This is the image module class
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	public class Imaging : ModuleBase
    {
		/// <summary>
		/// The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		/// This initializes the ImageModule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Imaging(CommandService service)
		{
			_service = service;
		}

		/// <summary>
		/// Flippedy flip!
		/// </summary>
		/// <param name="degrees">The degrees.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		[Command("flip")]
		[Remarks("Flips your Discord Avatar or another image specified through an URL")]
		public async Task FlipImage(int degrees = 888, string url = null)
		{
			await flipImage(degrees, url);
		}

		/// <summary>
		/// The task used inside FlipImage
		/// </summary>
		/// <param name="degrees">The degrees.</param>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
		public async Task flipImage(int degrees, string url)
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
				ImageCore.ImageCore core = new ImageCore.ImageCore();
				var img = await core.StartStreamAsync(Context.User);
				img.Rotate(degrees);
				await core.StopStreamAsync(Context.Message, img);
			}
			else
			{
				ImageCore.ImageCore core = new ImageCore.ImageCore();
				var img = await core.StartStreamAsync(Context.User, url);
				img.Rotate(degrees);
				await core.StopStreamAsync(Context.Message, img);
			}
		}

		/// <summary>
		/// Instagram Emulator 2017
		/// </summary>
		/// <param name="filter">The filter.</param>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("filter")]
		[Remarks("We all know Instagram to be honest")]
		public async Task filterImage(string filter = null, [Remainder] SocketUser user = null)
		{
			var task = Task.Run(async () =>
			{
				ImageCore.ImageCore core = new ImageCore.ImageCore();
				ImageSharp.Image<Rgba32> img = null;

				if (filter != "help")
				{
					if (user != null) img = await core.StartStreamAsync(user);
					else img = await core.StartStreamAsync(Context.User);
					img.Resize(500, 500);
				}

				Random rand = new Random();
				string[] randomFilters = new string[]
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
				await Context.Channel.SendMessageAsync($"Applyin filter = {filter}");
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
						embed.AddInlineField("Filter help:", "You can try any of these filters/operations to yours or another users avatar:\n" +
							"```sepia, vignette, polaroid, pixelate, oilpaint, lomograph, kodachrome, invert, grayscale, glow, sharpen, blue, dither, detectedges, colorblind, blackwhite, threshold or 'random' for a random selection.```");
						await Context.Channel.SendMessageAsync("", false, embed);
						return;
				}
				await core.StopStreamAsync(Context.Message, img);
			});
		}
    }
}