// UserEnter.cs
// This Class is used to provide a method around user related data which
// is getting stored in the bots own files.

using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using RicaBotpaw.Attributes;
using RicaBotpaw.Libs;
using RicaBotpaw.Modules.Data;

namespace RicaBotpaw.Modules.Users
{
    public class UserEnter : ModuleBase
    {
	    private DateTime _daily;
	    private CommandService _service;


	    public UserEnter(CommandService service)
	    {
		    _service = service;
	    }

		[Command("register", RunMode = RunMode.Async),RBRatelimit(1, 5, Measure.Seconds)]
	    [Remarks("Registers you into ricas new data logger!")]
	    public async Task EnterJson([Remainder] IUser u = null)
		{
			if (u == null) u = Context.User;


			var fileName = u.Id.ToString() + "_" + u.Username.ToString();
			var guid = Guid.NewGuid().ToString();
			_daily = DateTime.Now;

			DiscordUserJSON data = new DiscordUserJSON
			{
				UserId = u.Id,
				Username = u.Username,
				Money = 400,
				Tokens = 400,
				UserGuid = guid,
				Daily = _daily
			};

			using (StreamWriter file = File.CreateText($"./Data/users/{fileName}.rbuser"))
			{
				var fileText = JsonConvert.SerializeObject(data);
				var fileText1 = EncoderUtils.B64Encode(fileText);
				await file.WriteAsync(fileText1);
				await ReplyAsync($"User file {fileName}.rbuser created at /Data/users");
			}
	    }
	}
}