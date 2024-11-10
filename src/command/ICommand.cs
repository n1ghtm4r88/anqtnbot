using Telegram.Bot;

namespace anqtnbot.command;

public interface ICommand
{
	public string Name { get; }
	public string[] Aliases { get; }

	public Task ExecuteAsync(ITelegramBotClient botClient, Command command, CancellationToken cancellationToken);
}
