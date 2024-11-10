namespace anqtnbot.command;

public class CommandRegistry : ICommandRegistry
{
	private readonly Dictionary<string, ICommand> _commands = new();

	public void RegisterCommand(ICommand command)
	{
		_commands[command.Name.ToLower()] = command;

		foreach (var alias in command.Aliases)
		{
			_commands[alias.ToLower()] = command;
		}
	}

	public ICommand? GetCommand(string name)
	{
		_commands.TryGetValue(name.ToLower(), out var command);

		return command;
	}

	public IEnumerable<ICommand> GetAll() => _commands.Values.Distinct();
}
