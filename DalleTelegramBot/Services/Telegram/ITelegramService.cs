using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Services.Telegram;

internal interface ITelegramService
{
    Task<Message> SendMessageAsync(ChatId id, string text, CancellationToken cancellationToken = default);
    Task<Message> SendMessageAsync(ChatId id, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken = default);
    Task<Message> SendMessageAsync(ChatId id, string text, IReplyMarkup replyMarkup, ParseMode parseMode, CancellationToken cancellationToken = default);
    Task<Message> SendMessageAsync(ChatId id, string text, ParseMode parseMode, CancellationToken cancellationToken = default);

    Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, CancellationToken cancellationToken = default);
    Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken = default);
    Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, IReplyMarkup replyMarkup, ParseMode parseMode, CancellationToken cancellationToken = default);
    Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, ParseMode parseMode, CancellationToken cancellationToken = default);

    Task<Message> EditMessageAsync(ChatId id, int messageId, string text, CancellationToken cancellationToken = default);
    Task<Message> EditMessageAsync(ChatId id, int messageId, string text, InlineKeyboardMarkup replyMarkup, CancellationToken cancellationToken = default);
    Task<Message> EditMessageAsync(ChatId id, int messageId, string text, InlineKeyboardMarkup replyMarkup, ParseMode parseMode, CancellationToken cancellationToken = default);
    Task<Message> EditMessageAsync(ChatId id, int messageId, string text, ParseMode parseMode, CancellationToken cancellationToken = default);

    Task<Message> ForwardMessageAsync(ChatId to, ChatId from, int messageId, CancellationToken cancellationToken = default);

    Task<Message> SendDocumentAsync(ChatId id, string pathDoc, string caption, CancellationToken cancellationToken = default);
    Task<Message> SendDocumentAsync(ChatId id, string pathDoc, string caption, ParseMode parseMode, CancellationToken cancellationToken = default);

    Task DeleteMessageAsync(ChatId id, int messageId, CancellationToken cancellationToken = default);

    Task SendChatActionAsync(long userId, ChatAction chatAction, CancellationToken cancellationToken = default);

    Task SendMediaGroupAsync(long userId, int messageId, IEnumerable<IAlbumInputMedia> media, CancellationToken cancellationToken = default);

    Task AnswerCallbackQueryAsync(string callbackQueryId, CancellationToken cancellationToken = default);
    Task AnswerCallbackQueryAsync(string callbackQueryId, string text, CancellationToken cancellationToken = default);
}
