using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace RicaBotpaw
{
	/// <summary>
	/// The main file
	/// </summary>
	public class Program
    {
		// Convert our sync main to an async main.
		/// <summary>
		/// The async main method required for the bot
		/// </summary>
		/// <param name="args">The arguments.</param>
		public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

		/// <summary>
		/// The client
		/// </summary>
		private DiscordSocketClient _client;
		/// <summary>
		/// The commands
		/// </summary>
		private CommandHandler _commands;

		/// <summary>
		/// This starts our bot
		/// </summary>
		/// <returns></returns>
		public async Task Start()
        {
			_client = new DiscordSocketClient();
            _commands = new CommandHandler();

			await _client.LoginAsync(TokenType.Bot, "");
            await _client.StartAsync();

            _client.Log += Log;

            await _commands.Install(_client);

            await Task.Delay(-1);
        }

		/// <summary>
		/// Logs the specified MSG.
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		private Task Log(LogMessage msg)
        {
			var logger = Console.ForegroundColor;

			switch(msg.Severity)
			{
				case LogSeverity.Critical:
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case LogSeverity.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogSeverity.Warning:
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					break;
				case LogSeverity.Info:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case LogSeverity.Verbose:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogSeverity.Debug:
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
			}

			Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message}");
			Console.ForegroundColor = logger;
            return Task.CompletedTask;
        }
    }
}