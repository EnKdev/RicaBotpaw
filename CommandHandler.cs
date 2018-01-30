using System;
using System.Timers;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw
{
	/// <summary>
	///     This is the commandhandler where all modules get registered
	/// </summary>
	public class CommandHandler
	{
		/// <summary>
		///     The client
		/// </summary>
		private DiscordSocketClient _client;

		/// <summary>
		///     The CMDS
		/// </summary>
		private CommandService _cmds;

		/// <summary>
		/// a timer
		/// </summary>
		private static System.Timers.Timer aTimer;

		/// <summary>
		/// The status strings
		/// </summary>
		private string[] statusStrings = new string[]
		{
			"RubRub was here.",
			"Being cute",
			"RUBRUBRUBRUBRUBRUBRUBRUBRUB",
			"Visual Studio 2017",
			"For help, use ;help",
			"Need a cat? Try ;cat",
			"I have databases",
			"Omae wa mou! Shindeiru!",
			"NANI?!",
			"being the best achievement EnK_ has ever made in terms of development",
			"Hyet! Ha! *Screaming*",
			"with your neck"
		};

		/// <summary>
		/// The rand status
		/// </summary>
		private Random randStatus = new Random();

		/// <summary>
		///     This "installs" all modules into the handler and registers their commands
		/// </summary>
		/// <param name="c">The c.</param>
		/// <returns></returns>
		public async Task Install(DiscordSocketClient c)
		{
			_client = c;
			_cmds = new CommandService();

			await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());

			_client.MessageReceived += HandleCommand;
			_client.Ready += onReady;
			_client.UserJoined += onJoin;
			_client.UserLeft += onLeave;
		}

		/// <summary>
		///     This is the actual commandhandler
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns></returns>
		public async Task HandleCommand(SocketMessage s)
		{
			var msg = s as SocketUserMessage;
			if (msg == null) return;

			var context = new CommandContext(_client, msg);

			var argPos = 0;
			if (msg.HasStringPrefix(";", ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
			{
				var result = await _cmds.ExecuteAsync(context, argPos);

				if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ToString());
			}
		}

		/// <summary>
		///     When the bot is ready, this will be triggered
		/// </summary>
		/// <returns></returns>
		public async Task onReady()
		{
			await _client.SetGameAsync("For help, use ;help"); // Default first status.
			await CheckTime();
		}

		/// <summary>
		///     When a new user joins a guild, this will be triggered
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public async Task onJoin(SocketGuildUser user)
		{
			var channel = user.Guild.DefaultChannel;
			await channel.SendMessageAsync(user.Username + " has entered the server! Say hi!");
		}

		/// <summary>
		///     When a user leaves a guild, this will be triggered
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		public async Task onLeave(SocketGuildUser user)
		{
			var channel = user.Guild.DefaultChannel;
			await channel.SendMessageAsync(user.Username + " has left us alone! Parting is such a sweet sorrow...");
		}

		public async Task CheckTime()
		{
			var signalTime = DateTime.Now;
			aTimer = new System.Timers.Timer();
			aTimer.Interval = 300000; // 2.5 Minutes = 150000 (Testing)
			aTimer.Elapsed += OnTimedEvent;
			aTimer.AutoReset = true;
			aTimer.Enabled = true;
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"[StatusTimer] Timer has been started at {signalTime}. Statuses change each 5 (300 seconds) minutes now.");
			Console.ForegroundColor = ConsoleColor.White;
		}

		public async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
		{
			var signalTime = e.SignalTime;
			var randomIndex = randStatus.Next(statusStrings.Length);
			var text = statusStrings[randomIndex];
			await _client.SetGameAsync(text);
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"[StatusTimer] New status has been set at {signalTime}\n[StatusTimer] Current status: {text}");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}