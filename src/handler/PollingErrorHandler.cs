using Telegram.Bot;

namespace anqtnbot.handler;

public class PollingErrorHandler : IPollingErrorHandler
{
	public Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
	{
		Console.WriteLine(exception.Message);

		return Task.CompletedTask;
	}
}
