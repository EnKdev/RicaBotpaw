// RBConfig.cs
// This is the bot's general "Configuration" class which actually is just a class
// To store and get several things of information about the bot and which OS the bot is running on.

using System.Reflection;
using System.Runtime.Versioning;

namespace RicaBotpaw.Config
{
	/// <summary>
	///     This is the configuration class
	/// </summary>
	public class RBConfig
	{
		/// <summary>
		///     Contains the bots version
		/// </summary>
		/// <value>
		///     The bot version.
		/// </value>
		public static string BotVersion = "2.0.0-pre7";

		/// <summary>
		/// The bots sub version name
		/// </summary>
		/// <value>
		/// The sub version name
		/// </value>
		public static string BotSubVersionName = "Cold Whisp";

		/// <summary>
		///     Contains the bots build revision
		/// </summary>
		/// <value>
		///     The build revision.
		/// </value>
		public static string BuildRevision = "1011180042_RB_200pre7";

		/// <summary>
		/// Contains the .NET Core Version
		/// </summary>
		/// <value>
		/// The .NET Core Version
		/// </value>
		public static string NetCoreVersion =
			Assembly.GetEntryAssembly()?.GetCustomAttribute<TargetFrameworkAttribute>()?.FrameworkName;

		/// <summary>
		///     Contains the bots author
		/// </summary>
		/// <value>
		///     The bot author.
		/// </value>
		public static string BotAuthor = "EnK_";

		/// <summary>
		///     Contains the bots module count
		/// </summary>
		/// <value>
		///     The module count.
		/// </value>
		public static int MainModules = 6;

		/// <summary>
		///		Contains the bots extensive module count
		/// </summary>
		/// <value>
		///		The extension module count.
		/// </value>
		public static int ExtensionModules = 2;
	}
}