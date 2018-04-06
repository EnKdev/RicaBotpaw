using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

/*
 * System.Security.Cryptography Hashing Algorithms:
 * HMAC, HMACMD5, HMACSHA1, HMACSHA256, HMACSHA384, HMACSHA512, MD5, SHA1, SHA256, SHA384, SHA512
 */

/*
 * System.Security.Cryptography Signing Algorithms:
 * DSA
 */

/*
 * System.Security.Cryptography En-/De-crypting Algorithms:
 * AES, DES, Base64, RC2, Rijndael, RSA, TripleDES, 
 */

namespace RicaBotpaw.Libs
{
	public class Hasher
	{
		/*
		 * Hash-based Message Authentication Code (HMAC)
		 */

		public static async Task HashHMACMD5(IUserMessage msg, string msgToHash)
		{
			using (HMAC hmacmd5 = new HMACMD5())
			{
				var hash = hmacmd5.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [HMAC (MD5)]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashHMACSHA1(IUserMessage msg, string msgToHash)
		{
			using (HMAC hmacsha1 = new HMACSHA1())
			{
				var hash = hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [HMAC (SHA-1)]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashHMACSHA256(IUserMessage msg, string msgToHash)
		{
			using (HMAC hmacsha256 = new HMACSHA256())
			{
				var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [HMAC (SHA-256)]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashHMACSHA384(IUserMessage msg, string msgToHash)
		{
			using (HMAC hmacsha384 = new HMACSHA384())
			{
				var hash = hmacsha384.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [HMAC (SHA-384)]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashHMACSHA512(IUserMessage msg, string msgToHash)
		{
			using (HMAC hmacsha512 = new HMACSHA512())
			{
				var hash = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [HMAC (SHA-512)]:\n" + "`" + sb.ToString() + "`");
			}
		}

		/*
		 * MD5
		 */

		public static async Task HashMD5(IUserMessage msg, string msgToHash)
		{
			using (MD5 md5 = new MD5CryptoServiceProvider())
			{
				var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [MD5]:\n" + "`" + sb.ToString() + "`");
			}
		}

		/*
		 * Secure Hash Algorithm (SHA)
		 */

		public static async Task HashSHA1(IUserMessage msg, string messageToHash)
		{
			using (SHA1Managed sha1 = new SHA1Managed())
			{
				var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(messageToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {messageToHash}\n" + "Hashed output [SHA-1]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashSHA256(IUserMessage msg, string messageToHash)
		{
			using (SHA256Managed sha256 = new SHA256Managed())
			{
				var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(messageToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {messageToHash}\n" + "Hashed output [SHA-256]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashSHA384(IUserMessage msg, string messageToHash)
		{
			using (SHA384Managed sha384 = new SHA384Managed())
			{
				var hash = sha384.ComputeHash(Encoding.UTF8.GetBytes(messageToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {messageToHash}\n" + "Hashed output [SHA-384]:\n" + "`" + sb.ToString() + "`");
			}
		}

		public static async Task HashSHA512(IUserMessage msg, string msgToHash)
		{
			using (SHA512Managed sha512 = new SHA512Managed())
			{
				var hash = sha512.ComputeHash(Encoding.UTF8.GetBytes(msgToHash));
				var sb = new StringBuilder(hash.Length * 2);

				foreach (byte b in hash)
				{
					sb.Append(b.ToString("x2"));
				}

				await msg.Channel.SendMessageAsync($"Raw input: {msgToHash}\n" + "Hashed output [SHA-512]:\n" + "`" + sb.ToString() + "`");
			}
		}
	}
}