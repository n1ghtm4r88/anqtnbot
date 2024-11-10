namespace anqtnbot.config;

public interface IConfig
{
	public string ConnectionString { get; }
	public string Token { get; }
	public string EncryptionKey { get; }
}
