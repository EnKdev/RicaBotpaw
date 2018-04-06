using Discord;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using ImageSharp;

namespace RicaBotpaw.Libs
{
	public class ImageCore
	{
		private string randomString = "";
		
		public async Task<ImageSharp.Image<Rgba32>> StartStreamAsync(IUser user = null, string url = null, string path = null)
		{
			HttpClient httpClient = new HttpClient();

			HttpResponseMessage res = null;
			ImageSharp.Image<Rgba32> img = null;

			if (user != null)
			{
				try
				{
					res = await httpClient.GetAsync(user.GetAvatarUrl());
				}
				catch
				{
					res = await httpClient.GetAsync("https://discordapp.com/assets/1cbd08c76f8af6dddce02c5138971129.png");
				}

				Stream iStream = await res.Content.ReadAsStreamAsync();
				img = ImageSharp.Image.Load<Rgba32>(iStream);
				iStream.Dispose();
			}

			if (url != null)
			{
				res = await httpClient.GetAsync(url);
				Stream iStream2 = await res.Content.ReadAsStreamAsync();
				img = ImageSharp.Image.Load<Rgba32>(iStream2);
				iStream2.Dispose();
			}

			if (path != null)
			{
				Stream iStream3 = File.Open(path, FileMode.Open);
				img = ImageSharp.Image.Load<Rgba32>(iStream3);
				iStream3.Dispose();
			}

			return img;
		}

		public async Task StopStreamAsync(IUserMessage msg, ImageSharp.Image<Rgba32> img)
		{
			string input = "abcdefghijklmnopqrstuvwxyz0123456789";
			char ch;
			Random rand = new Random();

			for (int i = 0; i < 19; i++)
			{
				ch = input[rand.Next(0, input.Length)];
				randomString += ch;
			}

			if (img != null)
			{
				Stream oStream = new MemoryStream();
				img.SaveAsPng(oStream);
				oStream.Position = 0;
				var file = File.Create($"./images/{randomString}.png");

				await oStream.CopyToAsync(file);
				file.Dispose();

				await msg.Channel.SendFileAsync($"./images/{randomString}.png");

				File.Delete($"./images/{randomString}.png");
			}
		}
	}
}