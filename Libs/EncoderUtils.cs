using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace RicaBotpaw.Libs
{
    public class EncoderUtils
    {
		public static string B64Encode(string plainText)
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			return Convert.ToBase64String(plainTextBytes);
		}

	    public static string B64Decode(string b64EncodedData)
	    {
		    var b64EncodedBytes = Convert.FromBase64String(b64EncodedData);
		    return Encoding.UTF8.GetString(b64EncodedBytes);
	    }

	    public static void EncryptThenCompress(string input, string output, ICryptoTransform encryptor)
	    {
		    using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read))
		    {
				using (var outputStream = new FileStream(output, FileMode.Create, FileAccess.Write))
				using (var gZipStream = new GZipStream(outputStream, CompressionMode.Compress))
				using (var cryptoStream = new CryptoStream(gZipStream, encryptor, CryptoStreamMode.Write))
				{
					inputStream.CopyTo(cryptoStream);
				}
		    }
	    }

	    public static void DecompressThenDecrypt(string input, string output, ICryptoTransform decryptor)
	    {
		    using (var inputStream = new FileStream(input, FileMode.Open, FileAccess.Read))
		    {
				using (var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress))
				using (var cryptoStream = new CryptoStream(gZipStream, decryptor, CryptoStreamMode.Read))
				using (var outputStream = new FileStream(output, FileMode.Create, FileAccess.Write))
				{
					cryptoStream.CopyTo(outputStream);
				}
		    }
	    }
	}
}
