namespace RicaBotpaw.Logging
{
	public class ModStrings
	{
		public const string GuildNoConfigFile = "Your guild seems to have no configuration file.\n" +
		                                        "Since Update 1.9, it is now mandatory to have one so the bot knows what to execute and what not.\n" +
		                                        "You can create the configuration file with `;conf <publicmodule> <economyfeature> <gamblingfeature> <pollfeature> <imageflag> <adminflag> <gameflag>`\n" +
		                                        "For more help, please use `;chelp conf`\n" +
		                                        "Also please be aware that the inputs for the configuration must be either 0 or 1, where as 0 is disabled and 1 is enabled.";
		public const string PublicNotEnabled = "The public module is not enabled.";
		public const string EconomyNotEnabled = "The Economy feature is not enabled.";
		public const string GamblingNotEnabled = "The Gambling feature is not enabled.";
		public const string PollNotEnabled = "The poll feature is not enabled.";
		public const string ImagingNotEnabled = "The imaging module is not enabled.";
		public const string AdminNotEnabled = "The admin module is not enabled.";
		public const string GamesNotEnabled = "The game module is not enabled.";
	}
}