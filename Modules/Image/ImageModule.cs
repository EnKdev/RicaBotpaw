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

namespace RicaBotpaw.Modules.Image
{
    public class ImageModule : ModuleBase
    {
		private CommandService _service;

		public ImageModule(CommandService service)
		{
			_service = service;
		}

		[Command("flip")]
		[Remarks("Flips your Discord Avatar or another image specified through an URL")]
		public async Task FlipImage(int degrees = 888, string url = null)
		{
			await flipImage(degrees, url);
		}

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
    }
}