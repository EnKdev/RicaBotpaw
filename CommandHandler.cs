// CommandHandler.cs
// This class is the handler class for the entire bot.
// All of the bot's modules are getting registered in here, as well as 
// With extensive classes like custom typereaders
// This class can also be used to add for e.g Random Status Strings to the bot.

using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RicaBotpaw.Config;
using RicaBotpaw.TypeReaders;

namespace RicaBotpaw
{
	/// <summary>
	///     This is the commandhandler where all modules get registered
	/// </summary>
	public class CommandHandler
	{
		/// <summary>
		/// The client
		/// </summary>
		private DiscordSocketClient _client;

		/// <summary>
		/// The CMDS
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
			"Feeling random? Use ;random help",
			"I have databases",
			"Omae wa mou! Shindeiru!",
			"NANI?!",
			"being the best achievement EnK_ has ever made in terms of development",
			"Hyet! Ha! *Screaming*",
			"with your neck",
			"Facepalm Simulator 2018",
			"no u",
			"Simulator Simulator 2018",
			"Being a construction site",
			"Rut simulator 2018",
			"Being a spooky thing since 2018!",
			"#BlameRex"
		};

		/// <summary>
		/// The random status class instance the bot uses to pick it's random status message
		/// </summary>
		private Random randStatus = new Random();

		/// <summary>
		/// This "installs" all modules into the handler and registers their commands
		/// </summary>
		/// <param name="c">The client.</param>
		/// <returns></returns>
		public async Task Install(DiscordSocketClient c)
		{
			_client = c;
			_cmds = new CommandService();

			// TypeReaders
			_cmds.AddTypeReader<IGuild>(new RBGuildTypeReader<SocketGuild>()); // Thanks Tcb!

			// Modules
			await _cmds.AddModulesAsync(Assembly.GetEntryAssembly());

			_client.MessageReceived += HandleCommand;
			_client.Ready += onReady;
		}

		/// <summary>
		/// This is the actual commandhandler
		/// </summary>
		/// <param name="s">The socketmessage.</param>
		/// <returns></returns>
		public async Task HandleCommand(SocketMessage s)
		{
			var msg = s as SocketUserMessage;
			if (msg == null) return;

			var context = new CommandContext(_client, msg);

			var argPos = 0;
			if (msg.HasStringPrefix("rb!", ref argPos) || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
			{
				var result = await _cmds.ExecuteAsync(context, argPos);

				if (!result.IsSuccess) await context.Channel.SendMessageAsync(result.ToString());
			}
		}

		/// <summary>
		/// When the bot is ready, this will be triggered
		/// </summary>
		/// <returns></returns>
		public async Task onReady()
		{
			await _client.SetGameAsync($"Rica Botpaw {RBConfig.BotVersion} | {RBConfig.BotSubVersionName}"); // Default first status.
			await CheckTime();
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