using System;

namespace RicaBotpaw.Modules.Data
{
	public class mainTable // Edit this to the name of your main-table inside your database
	{
		public string UserId { get; set; }
		public string Username { get; set; }
		public string Rank { get; set; }
		public uint Tokens { get; set; }
		public int Money { get; set; }
		public int Level { get; set; }
		public int XP { get; set; }
	}
}