using anqtnbot.command;
using Telegram.Bot;

namespace anqtnbot.handler;

public interface ICommandHandler
{
	public Task HandleCommand(ITelegramBotClient botClient, Command command, CancellationToken token);
}
