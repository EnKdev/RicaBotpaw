using System;
using System.Threading.Tasks;

namespace RicaBotpaw
{
	// This is the class which adds the cooldown to the bot. It prevents users from triggering a command in the 5 second cooldown span.
	[Obsolete("This class is obsolete and shouldn't be used anymore.\nPlease refer to using the UserCooldown method which has gotten implemented in 2.0-pre3", true)]
	public class BotCooldown
	{
		public static bool isCooldownRunning = false; // Default, also that thing which adds the secure factor by running checks if the bot is on cooldown or not.
		private static System.Timers.Timer cooldownTimer;
		public static string cooldownMsg = "The bot is on cooldown! Please wait at least 5 seconds!";
		public static async Task Cooldown()
		{
			var signalTime = DateTime.Now;
			cooldownTimer = new System.Timers.Timer();
			cooldownTimer.Interval = 5000;
			cooldownTimer.Elapsed += OnTimedEvent;
			cooldownTimer.AutoReset = false;
			cooldownTimer.Enabled = true;
			isCooldownRunning = true;
		}

		public static async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
		{
			var signalTime = e.SignalTime;
			cooldownTimer.Enabled = false;
			isCooldownRunning = false;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("[Cooldown] Bot has thawed!");
			Console.ForegroundColor = ConsoleColor.White;
		}
	}
}