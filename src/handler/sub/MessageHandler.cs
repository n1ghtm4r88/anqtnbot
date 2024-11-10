using System.Text;
using anqtnbot.command;
using anqtnbot.database;
using anqtnbot.locale;
using anqtnbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace anqtnbot.handler.sub;

public class MessageHandler(
	
	Locale locale,
	CommandHandler commandHandler,
	IProvider provider,
	Encryption encryption,
	IDatabase database) : IUpdateHandler
{
	public async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
	{
		var message = update.Message!;
		var sender = message.From!.Id;

		if (message.Type != MessageType.Text)
		{
			await botClient.SendTextMessageAsync(
				
				sender,
				await locale.Get(sender, "OnlyTextMessage"),
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken
			);

			return;
		}

		var text = message.Text!;

		if (text.Length > 4000)
		{
			await database.RemoveExpectation(sender);
			await botClient.SendTextMessageAsync(
				
				sender,
				await locale.Get(sender, "MessageIsTooLong"),
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken
			);

			return;
		}

		if (text.StartsWith('/'))
		{
			var labelSplit = text[1..].Split(' ', 2);
			var commandName = labelSplit[0];
			var commandArgs = labelSplit.Length > 1 ? labelSplit[1] : null;

			var command = new Command(commandName, commandArgs, sender);

			await commandHandler.HandleCommand(botClient, command, cancellationToken);

			return;
		}

		if (!await provider.InExpectation(sender))
		{
			await botClient.SendTextMessageAsync(
				
				sender,
				await locale.Get(sender, "LinkToRecipient"),
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken
			);

			return;
		}

		var entities = message.Entities;
		var questionText = TextFormat.EscapeMarkdownV2(new StringBuilder(text));
		string messageText;

		if (entities != null)
		{
			var entity = entities.ToList().GroupBy(n => n.Offset).Select(g => g.Last()).ToList();

			questionText = entity.OrderByDescending(x => x.Offset).Aggregate(questionText, TextFormat.Format);
		}

		try
		{
			var recipient = await provider.GetRecipient(sender);

			messageText = await provider.IsResponse(sender)
			
				? await locale.Get(recipient, "NewAnswer")
				: await locale.Get(recipient, "NewQuestion");

			var encryptedData = encryption.Encrypt(sender);
			var keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
			{
				InlineKeyboardButton.WithCallbackData(await locale.Get(recipient, "Respond"), $"respond:{encryptedData}"),
				InlineKeyboardButton.WithCallbackData(await locale.Get(recipient, "Ban"), $"ban:{encryptedData}")
			});

			await botClient.SendTextMessageAsync(
				
				recipient,
				messageText + questionText,
				parseMode: ParseMode.MarkdownV2,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
			);
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception.Message);
		}
		finally
		{
			messageText = await provider.IsResponse(sender)
			
				? await locale.Get(sender, "AnswerSent")
				: await locale.Get(sender, "QuestionSent");

			await database.RemoveExpectation(sender);
			await botClient.SendTextMessageAsync(
				
				sender,
				messageText,
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken
			);
		}
	}
}
