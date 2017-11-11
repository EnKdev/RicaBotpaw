using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RicaBotpaw.Modules.Data
{
	public class moneyTable // Edit this to your money-table inside your database
	{
		public string UserId { get; set; }
		public int Money { get; set; }
		public int storeMoney { get; set; }
	}
}