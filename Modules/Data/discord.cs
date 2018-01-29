using System;

namespace RicaBotpaw.Modules.Data
{
	/// <summary>
	///     This is the main data class for the main table
	/// </summary>
	public class discord
	{
		/// <summary>
		///     Gets or sets the UserId
		/// </summary>
		/// <value>
		///     The UserId
		/// </value>
		public string UserId { get; set; }

		/// <summary>
		///     Gets or sets the Username
		/// </summary>
		/// <value>
		///     The Username
		/// </value>
		public string Username { get; set; }

		/// <summary>
		///     Gets or sets the Rank
		/// </summary>
		/// <value>
		///     The Rank
		/// </value>
		public string Rank { get; set; }

		/// <summary>
		///     Gets or sets the tokens
		/// </summary>
		/// <value>
		///     The Tokens
		/// </value>
		public uint Tokens { get; set; }

		/// <summary>
		///     Gets or sets the Level
		/// </summary>
		/// <value>
		///     The Level
		/// </value>
		public int Level { get; set; }

		/// <summary>
		///     Gets or sets the XP
		/// </summary>
		/// <value>
		///     The XP
		/// </value>
		public int XP { get; set; }

		/// <summary>
		///     Gets or sets the daily.
		/// </summary>
		/// <value>
		///     The daily.
		/// </value>
		public DateTime Daily { get; set; }
	}
}