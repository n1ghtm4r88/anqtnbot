namespace anqtnbot.locale;

using database;
using System.Resources;
using System.Globalization;

public class Locale(IProvider provider)
{
	public async Task<string> Get(long id, string name)
	{
		var language = await provider.GetLanguage(id);

		var resourceManager = new ResourceManager($"anqtnbot.resources.locale.{language}", typeof(Program).Assembly);
		var culture = new CultureInfo(language);

		return resourceManager.GetString(name, culture) ?? "";
	}
}
