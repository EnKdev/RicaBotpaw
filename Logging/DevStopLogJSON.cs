using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RicaBotpaw.Logging
{
	public partial class DevStopLog
	{
		[JsonProperty("stopReason")]
		public StopReasonJSON StopReason { get; set; }
	}

	public partial class StopReasonJSON
	{
		[JsonProperty("logId")]
		public string LogId { get; set; }

		[JsonProperty("case")]
		public long Case { get; set; }

		[JsonProperty("caseIdComment")]
		public string CIDComment { get; set; }

		[JsonProperty("r")]
		public string R { get; set; }
	}
}