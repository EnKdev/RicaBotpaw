namespace RicaBotpaw.Logging
{
	public class ModStrings
	{
		public const string GuildNoConfigFile = "Your guild seems to have no configuration file.\n" +
		                                        "Since Update 1.9, it is now mandatory to have one so the bot knows what to execute and what not.\n" +
		                                        "You can create the configuration file with `;conf <publicmodule> <economyfeature> <gamblingfeature> <pollfeature> <imageflag> <gameflag> <nsfwfeature> <sfwfeature>`\n" +
		                                        "For more help, please use `;confhelp`\n" +
		                                        "Also please be aware that the inputs for the configuration must be either 0 or 1, where as 0 is disabled and 1 is enabled.";
		public const string PublicNotEnabled = "The public module is not enabled.";
		public const string EconomyNotEnabled = "The Economy feature is not enabled.";
		public const string GamblingNotEnabled = "The Gambling feature is not enabled.";
		public const string PollNotEnabled = "The poll feature is not enabled.";
		public const string ImagingNotEnabled = "The imaging module is not enabled.";
		public const string GamesNotEnabled = "The game module is not enabled.";
		public const string ConfigHelp = "Since Update 1.9, the bot now has it's first breaking change.\n" +
		                                 "It is now required to have a configuration file for your guild. With this file, you as the Guild Administrator or Guild Owner can define how this bot behaves on your guild.\n" +
		                                 "If the notification about your guild not having a config file didn't help, let me show you how the command is executed.\n" +
		                                 "`Example usage: ;conf 1 1 0 1 1 0 0 1`\n" +
		                                 "NOTICE: Entering numbers higher than 1 will result in the module being disabled. So please stick to using a 1 or a 0 only.";
		public const string NSFWImageSearchNotEnabled =
				"You cannot search for images on this discord, or the Channel you tried to search the image in isn't marked as NSFW for this occasion.";
		public const string SFWImageSearchNotEnabled = "The clean image search feature is not enabled.";
	}
}