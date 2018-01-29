namespace RicaBotpaw.Modules.Data
{
	/// <summary>
	///     Class for the money table used for handling economical features
	/// </summary>
	public class moneydiscord
	{
		/// <summary>
		///     Get/Setter for user_id
		/// </summary>
		public string UserId { get; set; }

		/// <summary>
		///     Get/Setter for the amount of money one has
		/// </summary>
		public int Money { get; set; }

		/// <summary>
		///     Get/Setter for the amount of money being stored for future payments
		/// </summary>
		public int StoreMoney { get; set; }
	}
}