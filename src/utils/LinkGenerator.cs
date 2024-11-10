namespace anqtnbot.utils;

public static class LinkGenerator
{
	public static string Get(long id)
	{
		return $"t.me/share/url?url=t.me/anqtnbot?start={id}";
	}
}
