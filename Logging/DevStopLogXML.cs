﻿using System.Xml.Serialization;

namespace RicaBotpaw.Logging
{
	[XmlRoot(ElementName = "stopReason")]
	public class StopReasonXML
	{
		[XmlElement(ElementName = "logId")]
		public string LogId { get; set; }

		[XmlElement(ElementName = "case")]
		public long Case { get; set; }

		[XmlElement(ElementName = "caseHelp")]
		public string CaseHelp { get; set; }

		[XmlElement(ElementName = "r")]
		public string R { get; set; }
	}
}