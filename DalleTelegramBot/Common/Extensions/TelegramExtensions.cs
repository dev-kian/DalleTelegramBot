using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Common.Extensions;

internal static class TelegramExtensions
{
    public static long UserId(this Message message)
        => message.Chat.Id;

    public static long UserId(this CallbackQuery callbackQuery)
        => callbackQuery.From.Id;

    public static string EscapeMarkdownV2(this string text)
    {
        // Escape the dot character
        text = text.Replace(".", "\\.");

        // Escape the greater-than sign character
        text = text.Replace(">", "\\>");

        // Escape the less-than sign character
        text = text.Replace("<", "\\<");

        // Escape the underscore character
        text = text.Replace("_", "\\_");

        // Escape the hyphen character
        text = text.Replace("-", "\\-");

        // Escape the asterisk character
        text = text.Replace("*", "\\*");

        // Escape the backtick character
        text = text.Replace("`", "\\`");

        // Escape the single exclamation character
        text = text.Replace("!", "\\!");

        // Escape the square bracket characters
        text = Regex.Replace(text, @"([\[\]])", @"\$1");

        // Escape the parentheses characters
        text = Regex.Replace(text, @"([()])", @"\$1");

        return text;
    }
}
