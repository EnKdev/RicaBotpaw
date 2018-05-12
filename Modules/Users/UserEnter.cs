using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
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

	    [Command("register", RunMode = RunMode.Async)]
	    [Remarks("Registers you into ricas new data logger!")]
	    public async Task EnterJson([Remainder] IUser u = null)
	    {
		    if (BotCooldown.isCooldownRunning == false)
		    {
			    if (u == null)
			    {
				    u = Context.User;
				    var fileName = u.Id.ToString() + "_" + u.Username.ToString();
				    var guid = Guid.NewGuid().ToString();

					DiscordUserJSON data = new DiscordUserJSON
					{
						UserId = u.Id,
						Username = u.Username,
						Money = 400,
						Tokens = 400,
						UserGuid = guid
					};

				    using (StreamWriter file = File.CreateText($"./Data/users/{fileName}.rbuser"))
				    {
					    var fileText = JsonConvert.SerializeObject(data);
					    await file.WriteAsync(fileText);
					    await ReplyAsync($"User file {fileName}.rbuser created at /users");
				    }

				    await BotCooldown.Cooldown();
			    }
		    }
		    else
		    {
			    await ReplyAsync(BotCooldown.cooldownMsg);
		    }
	    }
	}
}