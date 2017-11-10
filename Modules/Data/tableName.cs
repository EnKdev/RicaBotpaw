using System;

namespace RicaBotpaw.Modules.Data
{
	public class tableName // You want to rename this if you are using a db
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