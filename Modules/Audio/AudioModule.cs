using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using Discord.Audio;

namespace RicaBotpaw.Modules.Audio
{
	public class AudioModule : ModuleBase
	{
		private Process CreateYTStream(string url)
		{
			Process currentSong = new Process();

			currentSong.StartInfo = new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = $"/C youtube-dl.exe -o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};

			currentSong.Start();
			return currentSong;
		}

		private CommandService _service;

		public AudioModule(CommandService service)
		{
			_service = service;
		}

		[Command("ytStream", RunMode = RunMode.Async)]
		[Remarks("Streams a song directly from youtube! (Very laggy tho. Better not use it until the bot is not selfhosted anymore")]
		public async Task play(string url)
		{
			IVoiceChannel channel = (Context.User as IVoiceState).VoiceChannel;
			IAudioClient client = await channel.ConnectAsync();

			var output = CreateYTStream(url).StandardOutput.BaseStream;
			var stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024);
			await output.CopyToAsync(stream);
			await stream.FlushAsync().ConfigureAwait(false);
		}
	}
}