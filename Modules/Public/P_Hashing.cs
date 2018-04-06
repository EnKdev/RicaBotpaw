using RicaBotpaw.Libs;
using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using RicaBotpaw.Logging;

namespace RicaBotpaw.Modules.Public
{
	[Group("hash")]
	[Remarks("For fun. DO NOT USE THIS TO SECURE YOUR DATA!")]
	public class Hashing : ModuleBase
	{
		private int featEnable;
		private int gNoticeSent;

		private CommandService _service;

		public Hashing(CommandService service)
		{
			_service = service;
		}

		private async Task CheckEnableFeatureModule([Remainder] IGuild g = null)
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
			if (mods.ModHash == 1)
			{
				featEnable = 1;
				return;
			}
			featEnable = 0;
		}

		/*
		 * Hash-based Message Authentication Code
		 */

		[Command("hmac-md5", RunMode = RunMode.Async)]
		[Remarks("Hashes your input into the Hash-based Message Authentication Code based on the Message Digest 5 hashing algorithm")]
		public async Task HMACMD5Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashHMACMD5(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("hmac-sha1", RunMode = RunMode.Async)]
		[Remarks("Hashes your input into the Hash-based Message Authentication Code based on the Secure Hash Algorithm 1")]
		public async Task HMACSHA1Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashHMACSHA1(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("hmac-sha256", RunMode = RunMode.Async)]
		[Remarks("Hashes your input into the Hash-based Message Authentication Code based on the Secure Hash Algorithm 256")]
		public async Task HMACSHA256Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashHMACSHA256(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("hmac-sha384", RunMode = RunMode.Async)]
		[Remarks("Hashes your input into the Hash-based Message Authentication Code based on the Secure Hash Algorithm 384")]
		public async Task HMACSHA384Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashHMACSHA384(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("hmac-sha512", RunMode = RunMode.Async)]
		[Remarks("Hashes your input into the Hash-based Message Authentication Code based on the Secure Hash Algorithm 512")]
		public async Task HMACSHA512Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashHMACSHA512(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		/*
		 * MD5
		 */

		[Command("md5", RunMode = RunMode.Async)]
		[Remarks("Hashes your input using the Message Digest 5 hashing algorithm")]
		public async Task MD5Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashMD5(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}
		
		/*
		 * Secure Hash Algorithm
		 */

		[Command("sha1", RunMode = RunMode.Async)]
		[Remarks("Hashes your input using the SHA-1 hashing algorithm")]
		public async Task SHA1Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashSHA1(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("sha256", RunMode = RunMode.Async)]
		[Remarks("Hashes your input using the SHA-256 hashing algorithm")]
		public async Task SHA256Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashSHA256(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("sha384", RunMode = RunMode.Async)]
		[Remarks("Hashes your input using the SHA-384 hashing algorithm")]
		public async Task SHA384Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashSHA384(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
				}
				else
				{
					gNoticeSent = 0;
					return;
				}
			}
		}

		[Command("sha512", RunMode = RunMode.Async)]
		[Remarks("Hashes your input using the SHA-512 hashing algorithm")]
		public async Task SHA512Hashing([Remainder] string input = null)
		{
			var g = Context.Guild as SocketGuild;
			await CheckEnableFeatureModule(g);

			if (featEnable == 1)
			{
				if (BotCooldown.isCooldownRunning == false)
				{
					if (input == null)
					{
						await Context.Channel.SendMessageAsync("Can't hash an empty input.");
					}
					else
					{
						await Hasher.HashSHA512(Context.Message, input);
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
					await ReplyAsync(ModStrings.HashingNotEnabled);
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