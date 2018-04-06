using Newtonsoft.Json;

namespace RicaBotpaw.Config
{
	public partial class Modules
	{
		[JsonProperty("_comment")]
		public string Comment { get; set; }

		[JsonProperty("guild")]
		public ulong Guild { get; set; }

		[JsonProperty("mod_pub")]
		public long ModPub { get; set; }

		[JsonProperty("feat_eco")]
		public long ModPubEco { get; set; }

		[JsonProperty("feat_eco_gmb")]
		public long ModPubEcoGmb { get; set; }

		[JsonProperty("feat_poll")]
		public long ModPubPoll { get; set; }

		[JsonProperty("mod_img")]
		public long ModImg { get; set; }

		[JsonProperty("mod_game")]
		public long ModGame { get; set; }

		[JsonProperty("feat_nsfw")]
		public long ModNSFW { get; set; }

		[JsonProperty("feat_sfw")]
		public long ModSFW { get; set; }

		[JsonProperty("feat_randimg")]
		public long ModRandImg { get; set; }

		[JsonProperty("feat_hash")]
		public long ModHash { get; set; }
	}
}