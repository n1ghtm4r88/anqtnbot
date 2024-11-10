namespace anqtnbot.config;

public class Config : IConfig
{
	public string ConnectionString { get; init; } = null!;
	public string Token { get; init; } = null!;
	public string EncryptionKey { get; init; } = null!;
}
