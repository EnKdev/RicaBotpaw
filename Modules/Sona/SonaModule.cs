using Discord;
using Discord.Commands;
using RicaBotpaw.Modules.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RicaBotpaw.Modules.Sona
{
	/// <summary>
	/// The SonaModule class
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	public class Sona : ModuleBase
	{
		/// <summary>
		/// The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		/// Initializes the sona module into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Sona(CommandService service)
		{
			_service = service;
		}


		/// <summary>
		/// Register your sona!
		/// </summary>
		/// <param name="user">The user.</param>
		/// <param name="name">The name.</param>
		/// <param name="age">The age.</param>
		/// <param name="spec">The species.</param>
		/// <param name="gend">The gender.</param>
		/// <param name="sex">The sexuality.</param>
		/// <returns></returns>
		[Command("register")]
		[Remarks("Registers your sona into the sona database")]
		public async Task RegisterSona(IUser user, string name, int age, string spec, string gend, string sex)
		{
			if (user == null)
			{
				Database.EnterUser(user);
			}
			else
			{
				Database.RegisterSona(user, name, age, spec, gend, sex);
				await ReplyAsync("Registered! :thumbsup::skin-tone-1:");
			}
		}

		/// <summary>
		/// Because fursonas are lovely beings~
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		[Command("sona")]
		[Remarks("Returns info about sona by a certain user")]
		public async Task SonaInfo([Remainder]IUser user)
		{
			var embed = new EmbedBuilder()
			{
				Color = new Color(109, 218, 37)
			};

			if (user == null)
			{
				user = Context.User;
			}

			var result = Database.GetSona(user);

			if (result.Count == null)
			{
				await ReplyAsync("You haven't registered a sona dear.");
			}
			else
			{
				embed.Description = (user.Username + "'s Sona is as followed:\n\n"
					+ ":arrow_right:" + "Sona: " + result.FirstOrDefault().SonaName + "\n"
					+ ":arrow_right:" + "Age: " + result.FirstOrDefault().Age + "\n"
					+ ":arrow_right:" + "Species: " + result.FirstOrDefault().Species + "\n"
					+ ":arrow_right:" + "Gender: " + result.FirstOrDefault().Gender + "\n"
					+ ":arrow_right:" + "Sexuality: " + result.FirstOrDefault().Sexuality);

				await Context.Channel.SendMessageAsync("", false, embed);
			}
		}
	}
}