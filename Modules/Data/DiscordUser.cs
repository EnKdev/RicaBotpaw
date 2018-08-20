using System;
using Newtonsoft.Json;

namespace RicaBotpaw.Modules.Data
{
	public partial class DiscordUserJSON
	{
		[JsonProperty("userID")]
		public ulong UserId { get; set; }

		[JsonProperty("username")]
		public string Username { get; set; }

		[JsonProperty("tokens")]
		public long Tokens { get; set; }

		[JsonProperty("userGUID")]
		public string UserGuid { get; set; }

		[JsonProperty("money")]
		public long Money { get; set; }

		[JsonProperty("daily")]
		public DateTime Daily { get; set; }
	}
}