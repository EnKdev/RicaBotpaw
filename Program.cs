using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace RicaBotpaw
{
    public class Program
    {
        // Convert our sync main to an async main.
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient _client;
        private CommandHandler _commands;

        public async Task Start()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandHandler();

			string token = "";


			await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            _client.Log += Log;

            await _commands.Install(_client);

            await Task.Delay(-1);
        }

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