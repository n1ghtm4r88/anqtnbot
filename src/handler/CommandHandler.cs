using anqtnbot.command;
using anqtnbot.locale;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace anqtnbot.handler;

public class CommandHandler(ICommandRegistry registry, Locale locale) : ICommandHandler
{
	public async Task HandleCommand(ITelegramBotClient botClient, Command command, CancellationToken cancellationToken)
	{
		var cmd = registry.GetCommand(command.Name);
		var sender = command.Sender;

		if (cmd != null)
		{
			await cmd.ExecuteAsync(botClient, command, cancellationToken);

			return;
		}

		await botClient.SendTextMessageAsync(
			
			sender,
			await locale.Get(sender, "CommandNotFound"),
			parseMode: ParseMode.MarkdownV2,
			cancellationToken: cancellationToken
		);
	}
}
