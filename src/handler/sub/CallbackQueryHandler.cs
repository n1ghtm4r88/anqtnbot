using anqtnbot.command;
using anqtnbot.command.standard;
using anqtnbot.database;
using anqtnbot.locale;
using anqtnbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace anqtnbot.handler.sub;

public class CallbackQueryHandler(
	
	StartCommand command,
	IDatabase database,
	Encryption encryption,
	Locale locale) : IUpdateHandler
{
	public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
	{
		var callbackQuery = update.CallbackQuery!;
		var data = callbackQuery.Data!;
		var sender = callbackQuery.From.Id;
		var message = callbackQuery.Message!;
		var messageId = message.MessageId;

		if (data.StartsWith("sl"))
		{
			command.MessageId = messageId;

			await database.AddUser(sender, data.Split(':')[1]);
			await command.ExecuteAsync(botClient, new Command(command.Name, null, sender), cancellationToken);

			return;
		}

		if (data.StartsWith("cancel"))
		{
			command.MessageId = messageId;

			await database.RemoveExpectation(sender);
			await command.ExecuteAsync(botClient, new Command(command.Name, null, sender), cancellationToken);

			return;
		}

		var encryptedData = data.Split(':')[1];
		long recipient;

		if (data.StartsWith("respond"))
		{
			command.IsResponse = true;

			recipient = encryption.Decrypt(encryptedData);

			await command.ExecuteAsync(botClient, new Command(command.Name, recipient.ToString(), sender), cancellationToken);
			await botClient.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken: cancellationToken);

			return;
		}

		var chatId = message.Chat.Id;
		recipient = sender;
		sender = encryption.Decrypt(encryptedData);

		if (data.StartsWith("ban"))
		{
			var keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
			{
				InlineKeyboardButton.WithCallbackData(await locale.Get(chatId, "Unban"), $"unban:{encryption.Encrypt(sender)}")
			});

			await database.AddBan(sender, recipient);
			await botClient.EditMessageReplyMarkupAsync(
				
				chatId,
				messageId,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
			);

			return;
		}

		if (data.StartsWith("unban"))
		{
			var keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
			{
				InlineKeyboardButton.WithCallbackData(await locale.Get(chatId, "Respond"), $"respond:{encryption.Encrypt(sender)}"),
				InlineKeyboardButton.WithCallbackData(await locale.Get(chatId, "Ban"), $"ban:{encryption.Encrypt(sender)}")
			});

			await database.RemoveBan(sender, recipient);
			await botClient.EditMessageReplyMarkupAsync(
				
				chatId,
				messageId,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
			);
		}
	}
}
