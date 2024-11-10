namespace anqtnbot.command;

public interface ICommandRegistry
{
	public void RegisterCommand(ICommand command);
	public ICommand? GetCommand(string name);
	public IEnumerable<ICommand> GetAll();
}
