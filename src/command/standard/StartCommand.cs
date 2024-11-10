using anqtnbot.database;
using anqtnbot.locale;
using anqtnbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace anqtnbot.command.standard;

public class StartCommand(Locale locale, IProvider provider, IDatabase database) : ICommand
{
	public int? MessageId { get; set; }
	public bool IsResponse { get; set; }

	public string Name => "start";
	public string[] Aliases => [];

	public async Task ExecuteAsync(ITelegramBotClient botClient, Command command, CancellationToken cancellationToken)
	{
		var args = command.Args;
		InlineKeyboardMarkup? keyboard;
		var sender = command.Sender;
		var recipient = Convert.ToInt64(args);

		if (args == null)
		{
			keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
			{
				InlineKeyboardButton.WithUrl(await locale.Get(sender, "ShareLink"), LinkGenerator.Get(sender))
			});

			if (MessageId.HasValue)
			{
				await botClient.EditMessageTextAsync(
					
					sender,
					(int) MessageId,
					(await locale.Get(sender, "YourLink")).Replace("{id}", sender.ToString()),
					parseMode: ParseMode.MarkdownV2,
					replyMarkup: keyboard,
					cancellationToken: cancellationToken
				);

				return;
			}

			await botClient.SendTextMessageAsync(
				
				sender,
				(await locale.Get(sender, "YourLink")).Replace("{id}", sender.ToString()),
				parseMode: ParseMode.MarkdownV2,
				replyMarkup: keyboard,
				cancellationToken: cancellationToken
			);

			return;
		}

		if (sender == recipient)
		{
			await botClient.SendTextMessageAsync(
				
				sender,
				await locale.Get(sender, "CannotUseOwnLink"),
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken
			);

			return;
		}

		if (await provider.IsBanned(sender, recipient))
		{
			await botClient.SendTextMessageAsync(
				
				sender,
				await locale.Get(sender, "IsBanned"),
				parseMode: ParseMode.MarkdownV2,
				cancellationToken: cancellationToken
			);

			return;
		}

		var name = IsResponse ? "SubmitYourAnswer" : "SubmitYourQuestion";
		keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton[]
		{
			InlineKeyboardButton.WithCallbackData(await locale.Get(sender, "Cancel"), "cancel")
		});

		await database.AddExpectation(sender, recipient, IsResponse);
		await botClient.SendTextMessageAsync(
			
			sender,
			await locale.Get(sender, name),
			parseMode: ParseMode.MarkdownV2,
			replyMarkup: keyboard,
			cancellationToken: cancellationToken
		);
	}
}
