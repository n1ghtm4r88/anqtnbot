namespace anqtnbot.command;

public class Command(string name, string? args, long sender)
{
	public readonly string Name = name;
	public readonly string? Args = args;
	public readonly long Sender = sender;
}
