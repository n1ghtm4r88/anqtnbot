using Microsoft.Extensions.Configuration;

namespace anqtnbot.config;

public class ConfigurationService
{
	public IConfigurationRoot Configuration { get; }

	public ConfigurationService()
	{
		var builder = new ConfigurationBuilder().SetBasePath(Path.GetFullPath(@"resources")).AddJsonFile("config.json");

		Configuration = builder.Build();
	}
}
