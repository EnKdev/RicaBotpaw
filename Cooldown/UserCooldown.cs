using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.Commands;

namespace RicaBotpaw.Cooldown
{
	public class UserCooldown : ModuleBase
	{
		public static IUser UserOnCooldown;
		public static List<IUser> UsersInCooldown = new List<IUser>();
		private static Timer CooldownTimer;
		public static bool UserIsInCooldown;

		public static async Task PutInCooldown([Remainder] IUser u)
		{
			UserOnCooldown = u;
			UsersInCooldown.Add(UserOnCooldown);
			Console.WriteLine($"Started individual countdown for User {UserOnCooldown.Username} [{UserOnCooldown.Id}]");
			await StartUserCooldown(u);
		}

		private static async Task StartUserCooldown(IUser u)
		{
			CooldownTimer = new Timer
			{
				Interval = 5000
			};
			CooldownTimer.Elapsed += PutOutOfCooldown;
			CooldownTimer.AutoReset = false;
			CooldownTimer.Enabled = true;
		}

		private static void PutOutOfCooldown(object sender, ElapsedEventArgs e)
		{
			CooldownStop(UserOnCooldown);
		}

		private static void CooldownStop(IUser u)
		{
			UsersInCooldown.Remove(u);
			Console.WriteLine($"Cooldown expired for User {UserOnCooldown.Username} [{UserOnCooldown.Id}]");
			UserOnCooldown = null;
		}
	}
}