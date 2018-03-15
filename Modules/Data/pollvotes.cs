namespace RicaBotpaw.Modules.Data
{
	public class PollVotes
	{
		public string UserId { get; set; }
		public int Vote { get; set; } // 0 if no, 1 if yes
	}
}