using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace anqtnbot.utils;

public static class TextFormat
{
	private static readonly char[] MarkdownV2Chars =
	[
		'_', '*', '`', '[', ']', '(', ')', '~', '>', '#', '+', '-', '.', '!'
	];

	public static StringBuilder EscapeMarkdownV2(StringBuilder message)
	{
		foreach (var symbol in MarkdownV2Chars)
		{
			var symbolString = symbol.ToString();

			message.Replace(symbolString, "\\" + symbolString);
		}

		return message;
	}

	public static StringBuilder Format(StringBuilder message, MessageEntity messageEntity)
	{
		return messageEntity.Type switch
		{
			MessageEntityType.Bold => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "*", "*"),
			MessageEntityType.Italic => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "_", "_"),
			MessageEntityType.Underline => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "__", "__"),
			MessageEntityType.Strikethrough => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "~", "~"),
			MessageEntityType.Spoiler => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "||", "||"),
			MessageEntityType.Code => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "```", "```"),
			MessageEntityType.Pre => FormatterToMarkdownV2(message, messageEntity.Offset, messageEntity.Length, "`", "`"),

			_ => message
		};
	}

	private static StringBuilder FormatterToMarkdownV2(StringBuilder message, int offset, int length, string symbol, string symbolEnd)
	{
		return message.Insert(offset + length, symbolEnd).Insert(offset, symbol);
	}
}
