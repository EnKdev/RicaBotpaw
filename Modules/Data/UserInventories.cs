using Newtonsoft.Json;

namespace RicaBotpaw.Modules.Data
{
	public partial class UserInventories
	{
		[JsonProperty("userID")]
		public ulong UserId { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("inventory")]
		public Inventory Inventory { get; set; }
	}

	public partial class Inventory
	{
		[JsonProperty("badges")]
		public string[] Badges { get; set; }

		[JsonProperty("consumables")]
		public string[] Consumables { get; set; }

		[JsonProperty("titles")]
		public string[] Titles { get; set; }
	}
}