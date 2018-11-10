// DevStopLogJSON.cs
// This class contains each needed JSON property so a devstop log file
// In Json Format under the file extension .rblog can be written.

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