using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Diagnostics;
using Discord.Audio;

namespace RicaBotpaw.Modules.Audio
{
	/// <summary>
	/// This is the audio class
	/// </summary>
	/// <seealso cref="Discord.Commands.ModuleBase" />
	public class Audio : ModuleBase
	{
		/// <summary>
		/// Creates the yt stream.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
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

		/// <summary>
		/// The service
		/// </summary>
		private CommandService _service;

		/// <summary>
		/// This registers the AudioModule into the commandhandler
		/// </summary>
		/// <param name="service">The service.</param>
		public Audio(CommandService service)
		{
			_service = service;
		}

		/// <summary>
		/// Youtube Audio Streaming!
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <returns></returns>
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