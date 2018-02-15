using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RicaBotpaw.Config
{
	public partial class ServerModulesConfig
	{
		[JsonProperty("_comment")]
		public string Comment { get; set; }

		[JsonProperty("modules")]
		public Modules Modules { get; set; }
	}

	public partial class Modules
	{
		[JsonProperty("_comment")]
		public string Comment { get; set; }

		[JsonProperty("guild")]
		public ulong Guild { get; set; }

		[JsonProperty("mod_pub")]
		public long ModPub { get; set; }

		[JsonProperty("mod_pub_eco")]
		public long ModPubEco { get; set; }

		[JsonProperty("mod_pub_eco_gmb")]
		public long ModPubEcoGmb { get; set; }

		[JsonProperty("mod_pub_poll")]
		public long ModPubPoll { get; set; }

		[JsonProperty("mod_img")]
		public long ModImg { get; set; }

		[JsonProperty("mod_adm")]
		public long ModAdm { get; set; }

		[JsonProperty("mod_game")]
		public long ModGame { get; set; }

	}

	public partial class ServerModulesConfig
	{
		public static ServerModulesConfig FromJson(string json) =>
			JsonConvert.DeserializeObject<ServerModulesConfig>(json, RicaBotpaw.Config.Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this ServerModulesConfig self) =>
			JsonConvert.SerializeObject(self, RicaBotpaw.Config.Converter.Settings);
	}

	public class Converter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None
		};
	}
}