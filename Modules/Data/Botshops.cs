using Newtonsoft.Json;

namespace RicaBotpaw.Modules.Data
{
	public partial class Botshops
	{
		[JsonProperty("shopName")]
		public string ShopName { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("contingent")]
		public Contingent Contingent { get; set; }
	}

	public partial class Contingent
	{
		[JsonProperty("item")]
		public Item Item { get; set; }
	}

	public partial class Item
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("isLimited")]
		public bool IsLimited { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("desc")]
		public string Desc { get; set; }

		[JsonProperty("cost")]
		public long Cost { get; set; }

		[JsonProperty("limitAmount", NullValueHandling = NullValueHandling.Ignore)]
		public long? LimitAmount { get; set; }

		[JsonProperty("reqTokens", NullValueHandling = NullValueHandling.Ignore)]
		public long? ReqTokens { get; set; }
	}
}