using Discord.WebSocket;
using System;

namespace RicaBotpaw.Modules.Data
{
	public class XP
	{
		public static int returnXP(SocketMessage msg)
		{
			Random rand = new Random();
			var msgCount = msg.Content.Length;
			var xp = rand.Next(msgCount / 3);
			return xp;
		}

		public static int calculateNextLevel(int currentLevel)
		{
			var calc = Math.Pow(currentLevel + 1, 3);
			var calc2 = Convert.ToInt32(calc);
			return calc2;
		}
	}
}