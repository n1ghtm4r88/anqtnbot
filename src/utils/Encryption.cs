using anqtnbot.config;

namespace anqtnbot.utils;

using System.Text;
using System.Security.Cryptography;

public class Encryption(IConfig config)
{
	public string Encrypt(long value)
	{
		var aes = GetAes();
		using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
		var bytes = Encoding.UTF8.GetBytes(value.ToString());

		return Convert.ToBase64String(aes.IV.Concat(encryptor.TransformFinalBlock(bytes, 0, bytes.Length)).ToArray());
	}

	public long Decrypt(string value)
	{
		var fullCipher = Convert.FromBase64String(value);
		var aes = GetAes();
		using var decryptor = aes.CreateDecryptor(aes.Key, fullCipher.Take(16).ToArray());
		var bytes = fullCipher.Skip(16).ToArray();

		return long.Parse(Encoding.UTF8.GetString(decryptor.TransformFinalBlock(bytes, 0, bytes.Length)));
	}

	private Aes GetAes()
	{
		var aes = Aes.Create();

		aes.GenerateIV();

		aes.Key = Convert.FromBase64String(config.EncryptionKey);
		aes.Mode = CipherMode.CBC;
		aes.Padding = PaddingMode.PKCS7;

		return aes;
	}
}
