using System;

namespace RicaBotpaw.Modules.Data
{
	/// <summary>
	/// The poll table class
	/// </summary>
	public class poll
	{
		/// <summary>
		/// Gets or sets the question.
		/// </summary>
		/// <value>
		/// The question.
		/// </value>
		public string Question { get; set; }

		/// <summary>
		/// Gets or sets the yes votes.
		/// </summary>
		/// <value>
		/// The yes votes.
		/// </value>
		public int YesVotes { get; set; }

		/// <summary>
		/// Gets or sets the no votes.
		/// </summary>
		/// <value>
		/// The no votes.
		/// </value>
		public int NoVotes { get; set; }

		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		public string UserId { get; set; }
	}
}