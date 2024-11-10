using Telegram.Bot;
using Telegram.Bot.Types;

namespace anqtnbot.handler;

public interface IUpdateHandler
{
	public Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken token);
}
