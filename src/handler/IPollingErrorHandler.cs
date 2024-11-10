using Telegram.Bot;

namespace anqtnbot.handler;

public interface IPollingErrorHandler
{
	public Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken token);
}
