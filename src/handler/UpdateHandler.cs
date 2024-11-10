using anqtnbot.command;
using anqtnbot.command.standard;
using anqtnbot.database;
using anqtnbot.handler.sub;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace anqtnbot.handler;

public class UpdateHandler(
	
	IProvider provider,
	CallbackQueryHandler callbackQueryHandler,
	ChangeLanguageCommand changeLanguageCommand,
	MessageHandler messageHandler) : IUpdateHandler
{
	public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
	{
		var callbackQuery = update.CallbackQuery;
		var sender = update.Message?.Chat.Id ?? callbackQuery!.From.Id;

		if (!await provider.IsRegistered(sender))
		{
			if (callbackQuery != null)
			{
				await callbackQueryHandler.HandleUpdate(botClient, update, cancellationToken);

				return;
			}

			changeLanguageCommand.CanCancel = false;
			
			await changeLanguageCommand.ExecuteAsync(botClient, new Command("changelanguage", null, sender), cancellationToken);

			return;
		}

		if (update.Type == UpdateType.Message)
		{
			await messageHandler.HandleUpdate(botClient, update, cancellationToken);

			return;
		}

		await callbackQueryHandler.HandleUpdate(botClient, update, cancellationToken);
	}
}