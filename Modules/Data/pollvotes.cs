using System;

namespace  RicaBotpaw.Modules.Data
{
	/// <summary>
	/// The get/set class for getting users who voted on a poll
	/// </summary>
	public class PollVotes
	{
		/// <summary>
		/// Gets or sets the user identifier.
		/// </summary>
		/// <value>
		/// The user identifier.
		/// </value>
		public string UserId { get; set; }

		/// <summary>
		/// Gets or sets the vote.
		/// </summary>
		/// <value>
		/// The vote.
		/// </value>
		public int Vote { get; set; } // 0 if no, 1 if yes
	}
}