using anqtnbot.locale;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace anqtnbot.command.standard;

public class ChangeLanguageCommand(Locale locale) : ICommand
{
	public bool CanCancel { get; set; } = true;
	
	public string Name => "changelanguage";
	public string[] Aliases => ["changelang", "chlang", "cl", "change-language", "change-lang"];

	public async Task ExecuteAsync(ITelegramBotClient botClient, Command command, CancellationToken cancellationToken)
	{
		var sender = command.Sender;
		var languages = new InlineKeyboardMarkup([
			[
				InlineKeyboardButton.WithCallbackData("🇺🇸", "sl:en"),
				InlineKeyboardButton.WithCallbackData("🇷🇺", "sl:ru")
			],
			CanCancel ? [InlineKeyboardButton.WithCallbackData(await locale.Get(sender, "Cancel"), "cancel")] : []
		]);

		await botClient.SendTextMessageAsync(
			
			sender,
			"*Select language* / *Выберите язык*",
			parseMode: ParseMode.MarkdownV2,
			replyMarkup: languages,
			cancellationToken: cancellationToken
		);
	}
}
