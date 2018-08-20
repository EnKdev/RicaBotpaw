// I_ImageSearch.cs
// The image searching subclass of ImageModule.cs
// This is to split the image module into files so that the subclasses have their own file.

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RicaBotpaw.Logging;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using RicaBotpaw.Cooldown;

namespace RicaBotpaw.Modules.Image
{
	public class ImageSearch : ModuleBase
	{
		private CommandService _service;

		public ImageSearch(CommandService service)
		{
			_service = service;
		}

		private int modEnableNSFW;
		private int modEnableSFW;
		private int gNoticeSent;
		private int maxTries1;
		private int maxTries2;

		private async Task CheckNSFWFeatureEnabled([Remainder] IGuild g = null)
		{
			if (g == null) g = Context.Guild;

			if (!File.Exists($"./Data/serv_configs/{g.Id.ToString()}_config.rconf"))
			{
				await ReplyAsync(ModStrings.GuildNoConfigFile);
				gNoticeSent = 1;
				return;
			}

			var fileText = File.ReadAllText($"./Data/serv_configs/{g.Id.ToString()}_config.rconf");
			var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText);

			if (mods.Guild != g.Id)
			{
				await ReplyAsync(
					"Specified Guild ID doesn't match saved Guild ID in config file."); // This should actually never happen unless you specified another guild... however that would work.
				return;
			}
			if (mods.ModNSFW == 1)
			{
				modEnableNSFW = 1;
				return;
			}
			modEnableNSFW = 0;
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

		private async Task CheckSFWFeatureEnabled([Remainder] IGuild g = null)
		{
			if (g == null) g = Context.Guild;

			if (!File.Exists($"./Data/serv_configs/{g.Id.ToString()}_config.rconf"))
			{
				await ReplyAsync(ModStrings.GuildNoConfigFile);
				gNoticeSent = 1;
				return;
			}

			var fileText = File.ReadAllText($"./Data/serv_configs/{g.Id.ToString()}_config.rconf");
			var mods = JsonConvert.DeserializeObject<Config.Modules>(fileText);

			if (mods.Guild != g.Id)
			{
				await ReplyAsync(
					"Specified Guild ID doesn't match saved Guild ID in config file."); // This should actually never happen unless you specified another guild... however that would work.
				return;
			}
			if (mods.ModSFW == 1)
			{
				modEnableSFW = 1;
				return;
			}
			modEnableSFW = 0;
		}

		[Command("e621", RunMode = RunMode.Async)]
		[Remarks(
			"Searches for an image based on tags you give it. If no tags exist, it searches by a default tag queue. Has to be called inside an NSFW marked channel")]
		[RequireNsfw]
		public async Task GetNSFWImage([Remainder] string input)
		{
			var user = Context.User as SocketUser;
			var g = Context.Guild as SocketGuild;
			await CheckNSFWFeatureEnabled(g);
			await CheckIfUserIsOnCooldown(user);

			if (modEnableNSFW == 1)
			{
				if (UserCooldown.UserIsInCooldown == false)
				{
					GetImage:
					using (var client = new HttpClient(new HttpClientHandler
					{
						AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
					})) // Webbrowser
					{
						if (input == null)
						{
							await ReplyAsync("Please enter a queue to search for!");
							return;
						}

						client.DefaultRequestHeaders.Add("User-Agent", "RicaBotpaw/2.0.0-pre3 (by EnK_ on e621)");
						string websiteUrl = "https://e621.net/post/index.json?tags=" + input + "%20order:random+rating:e&limit=1";
						client.BaseAddress = new Uri(websiteUrl);
						HttpResponseMessage res = client.GetAsync("").Result;
						res.EnsureSuccessStatusCode();
						string result = await res.Content.ReadAsStringAsync();
						result = result.TrimStart(new char[] {'['}).TrimEnd(new char[] {']'});

						try
						{
							var json = JObject.Parse(result);

							string YiffImage = json["file_url"].ToString();
							string Tags = json["tags"].ToString();

							if (Tags.Contains("bestiality") || Tags.Contains("cub") || Tags.Contains("child") || Tags.Contains("equine") ||
							    Tags.Contains("feral") || Tags.Contains("gumball") || Tags.Contains("horse") || Tags.Contains("human") ||
							    Tags.Contains("jasonafex") || Tags.Contains("jay_naylor") || Tags.Contains("manyakis") ||
							    Tags.Contains("micro_on_macro") || Tags.Contains("my_little_pony") || Tags.Contains("mlp") || Tags.Contains("peeing") ||
							    Tags.Contains("rating:safe") || Tags.Contains("r34") || Tags.Contains("scat") ||
							    Tags.Contains("size_difference") || Tags.Contains("type:swf") || Tags.Contains("vore") ||
							    Tags.Contains("watersports")) // Restricted tags
							{
								if (maxTries1 == 5)
								{
									await Context.Channel.SendMessageAsync(
										"Maximum tries exceeded. Please try again with a different queue or tags");
									maxTries1 = 0;
									return;
								}
								else
								{
									maxTries1++;
									await Context.Channel.SendMessageAsync("Generated an invalid image, please wait while we're retrying...");
									goto GetImage;
								}
							}

							var msg = await user.GetOrCreateDMChannelAsync();
							await msg.SendMessageAsync($"Here is your yiff! [Tags: {input}]\n" + YiffImage);
							await UserCooldown.PutInCooldown(user);
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							await Context.Channel.SendMessageAsync("API request failed. Please try again.");

							var application = await Context.Client.GetApplicationInfoAsync();
							var ownerNotification = await application.Owner.GetOrCreateDMChannelAsync();

							var embed = new EmbedBuilder()
							{
								Color = new Color(255, 0, 0)
							};

							embed.Description = $"Failed to send Yiff to {user.Username}#{user.Discriminator}, Tags: {input}";
							embed.WithFooter(
								new EmbedFooterBuilder().WithText("E621/E926 API Request failed!"));

							await ownerNotification.SendMessageAsync("", false, embed);
							await UserCooldown.PutInCooldown(user);
						}
					}
				}
			}
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.NSFWImageSearchNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("e926", RunMode = RunMode.Async)]
		[Remarks("Same as ;e621, but just clean")]
		public async Task GetSFWImage([Remainder] string input)
		{
			var user = Context.User as SocketUser;
			var ch = Context.Channel as SocketChannel;
			var g = Context.Guild as SocketGuild;
			await CheckSFWFeatureEnabled(g);
			await CheckIfUserIsOnCooldown(user);
			var maxTries = 0;

			if (modEnableSFW == 1)
			{
				if (UserCooldown.UserIsInCooldown == false)
				{
					GetImage:
					using (var client = new HttpClient(new HttpClientHandler
					{
						AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
					})) // Webbrowser
					{
						if (input == null)
						{
							await ReplyAsync("Please enter a queue to search for!");
							return;
						}

						client.DefaultRequestHeaders.Add("User-Agent", "RicaBotpaw/2.0.0-pre3 (by EnK_ on e621)");
						string websiteUrl = "https://e926.net/post/index.json?tags=" + input + "%20order:random&limit=1";
						client.BaseAddress = new Uri(websiteUrl);
						HttpResponseMessage res = client.GetAsync("").Result;
						res.EnsureSuccessStatusCode();
						string result = await res.Content.ReadAsStringAsync();
						result = result.TrimStart(new char[] { '[' }).TrimEnd(new char[] { ']' });

						try
						{
							var json = JObject.Parse(result);

							string FurImage = json["file_url"].ToString();
							string Tags = json["tags"].ToString();

							if (Tags.Contains("bestiality") || Tags.Contains("cub") || Tags.Contains("child") || Tags.Contains("equine") ||
								Tags.Contains("feral") || Tags.Contains("gumball") || Tags.Contains("horse") || Tags.Contains("human") ||
								Tags.Contains("jasonafex") || Tags.Contains("jay_naylor") || Tags.Contains("manyakis") ||
								Tags.Contains("micro_on_macro") || Tags.Contains("my_little_pony") || Tags.Contains("mlp") || Tags.Contains("peeing") ||
								Tags.Contains("rating:safe") || Tags.Contains("r34") || Tags.Contains("scat") ||
								Tags.Contains("size_difference") || Tags.Contains("sex") || Tags.Contains("type:swf") || Tags.Contains("vore") ||
								Tags.Contains("watersports")) // Restricted tags
							{
								if (maxTries2 == 5)
								{
									await Context.Channel.SendMessageAsync(
										"Maximum tries exceeded. Please try again with a different queue or tags");
									maxTries2 = 0;
									return;
								}
								else
								{
									maxTries2++;
									await Context.Channel.SendMessageAsync("Generated an invalid image, please wait while we're retrying...");
									goto GetImage;
								}
							}
							await Context.Channel.SendMessageAsync(user.Mention + $", Here is your image! Tags: [{input}]\n" + FurImage);
							await UserCooldown.PutInCooldown(user);
						}
						catch (Exception e)
						{
							Console.WriteLine(e);
							await Context.Channel.SendMessageAsync("API request failed. Please try again.");

							var application = await Context.Client.GetApplicationInfoAsync();
							var ownerNotification = await application.Owner.GetOrCreateDMChannelAsync();

							var embed = new EmbedBuilder()
							{
								Color = new Color(255, 0, 0)
							};

							embed.Description = $"Failed to send a picture to Channel {ch.Id} in Guild {g.Name}, Tags: {input}";
							embed.WithFooter(
								new EmbedFooterBuilder().WithText("E621/E926 API Request failed!"));

							await ownerNotification.SendMessageAsync("", false, embed);
							await UserCooldown.PutInCooldown(user);
						}
					}
				}
			}
			else
			{
				if (gNoticeSent == 0)
				{
					await ReplyAsync(ModStrings.SFWImageSearchNotEnabled);
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