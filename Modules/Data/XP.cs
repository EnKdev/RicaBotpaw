using System;
using Discord.WebSocket;

namespace RicaBotpaw.Modules.Data
{
	/// <summary>
	///     This is the class handled for xp and the level system
	/// </summary>
	public class XP
	{
		/// <summary>
		///     This calculates the XP given out based on the length of a message
		/// </summary>
		/// <param name="msg">The MSG.</param>
		/// <returns></returns>
		public static int returnXP(SocketMessage msg)
		{
			var rand = new Random();
			var msgCount = msg.Content.Length;
			var xp = rand.Next(msgCount / 3);
			return xp;
		}

		/// <summary>
		///     This calculates the XP you need for your next level
		/// </summary>
		/// <param name="currentLevel">The current level.</param>
		/// <returns></returns>
		public static int calculateNextLevel(int currentLevel)
		{
			var calc = Math.Pow(currentLevel + 1, 3);
			var calc2 = Convert.ToInt32(calc);
			return calc2;
		}
	}
}