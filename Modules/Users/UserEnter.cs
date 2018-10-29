using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Cooldown;
using RicaBotpaw.Libs;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules.Users
{
    public class UserEnter : ModuleBase
    {
	    private long _userid;
	    private long _tokens;
	    private long _money;
	    private string _username;
	    private string _customrank;
	    private Guid _guid;
	    private CommandService _service;

	    public UserEnter(CommandService service)
	    {
		    _service = service;
	    }

		private async Task CheckIfUserIsOnCooldown([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;

			if (UserCooldown.UsersInCooldown.Contains(u))
			{
				UserCooldown.UserIsInCooldown = true;
				ReplyAsync("You're in cooldown! Please wait 5 seconds!");
			}
			else
			{
				UserCooldown.UserIsInCooldown = false;
			}
		}

		[Command("register", RunMode = RunMode.Async)]
	    [Remarks("Registers you into ricas new data logger!")]
	    public async Task EnterJson([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;
			var u1 = Context.Message.Author;
			await CheckIfUserIsOnCooldown(u1);

		    if (UserCooldown.UserIsInCooldown == false)
		    {
				    var fileName = u.Id.ToString() + "_" + u.Username.ToString();
				    var guid = Guid.NewGuid().ToString();

					DiscordUserJSON data = new DiscordUserJSON
					{
						UserId = u.Id,
						Username = u.Username,
						Money = 400,
						Tokens = 400,
						UserGuid = guid,
						Daily = DateTime.Now
					};

				    using (StreamWriter file = File.CreateText($"./Data/users/{fileName}.rbuser"))
				    {
					    var fileText = JsonConvert.SerializeObject(data);
					    var fileText1 = EncoderUtils.B64Encode(fileText);
					    await file.WriteAsync(fileText1);
					    await ReplyAsync($"User file {fileName}.rbuser created at /Data/users");
				    }

			    UserCooldown.PutInCooldown(u1);
		    }
	    }
	}
}