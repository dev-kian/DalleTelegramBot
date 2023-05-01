using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Services.Telegram;

internal class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _botClient;
    public TelegramService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task<Message> SendMessageAsync(ChatId id, string text, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, cancellationToken: cancellationToken);
    }
    public async Task<Message> SendMessageAsync(ChatId id, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
    public async Task<Message> SendMessageAsync(ChatId id, string text, IReplyMarkup replyMarkup, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, parseMode: parseMode, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
    public async Task<Message> SendMessageAsync(ChatId id, string text, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, parseMode: parseMode, cancellationToken: cancellationToken);
    }

    public async Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, replyToMessageId: replyToMessageId, cancellationToken: cancellationToken);
    }
    public async Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, IReplyMarkup replyMarkup, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, replyMarkup: replyMarkup, replyToMessageId: replyToMessageId, cancellationToken: cancellationToken);
    }
    public async Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, IReplyMarkup replyMarkup, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, replyToMessageId: replyToMessageId, parseMode: parseMode, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
    public async Task<Message> ReplyMessageAsync(ChatId id, int replyToMessageId, string text, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        return await _botClient.SendTextMessageAsync(id, text, replyToMessageId: replyToMessageId, parseMode: parseMode, cancellationToken: cancellationToken);
    }


    public async Task<Message> EditMessageAsync(ChatId id, int messageId, string text, CancellationToken cancellationToken = default)
    {
        return await _botClient.EditMessageTextAsync(id, messageId, text, cancellationToken: cancellationToken);
    }
    public async Task<Message> EditMessageAsync(ChatId id, int messageId, string text, InlineKeyboardMarkup replyMarkup, CancellationToken cancellationToken = default)
    {
        return await _botClient.EditMessageTextAsync(id, messageId, text, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }
    public async Task<Message> EditMessageAsync(ChatId id, int messageId, string text, InlineKeyboardMarkup replyMarkup, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        return await _botClient.EditMessageTextAsync(id, messageId, text, parseMode: parseMode, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
    }

    public async Task<Message> EditMessageAsync(ChatId id, int messageId, string text, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        return await _botClient.EditMessageTextAsync(id, messageId, text, parseMode: parseMode, cancellationToken: cancellationToken);
    }

    public async Task<Message> ForwardMessageAsync(ChatId to, ChatId from, int messageId, CancellationToken cancellationToken = default)
    {
        return await _botClient.ForwardMessageAsync(to, from, messageId, cancellationToken: cancellationToken);
    }

    public async Task<Message> SendDocumentAsync(ChatId id, string pathDoc, string caption, CancellationToken cancellationToken = default)
    {
        return await SendDocumentAsync(id, pathDoc, caption, ParseMode.Html, cancellationToken);
    }

    public async Task<Message> SendDocumentAsync(ChatId id, string pathDoc, string caption, ParseMode parseMode, CancellationToken cancellationToken = default)
    {
        using (var stream = new FileStream(pathDoc, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            var file = new InputFile(stream, Path.GetFileName(pathDoc));
            return await _botClient.SendDocumentAsync(id, file, caption: caption, parseMode: parseMode, cancellationToken: cancellationToken);
        }
    }

    public async Task DeleteMessageAsync(ChatId id, int messageId, CancellationToken cancellationToken = default)
    {
        await _botClient.DeleteMessageAsync(id, messageId, cancellationToken);
    }

    public async Task SendChatActionAsync(long userId, ChatAction chatAction, CancellationToken cancellationToken = default)
    {
        await _botClient.SendChatActionAsync(userId, chatAction, cancellationToken: cancellationToken);
    }

    public async Task SendMediaGroupAsync(long userId, int messageId, IEnumerable<IAlbumInputMedia> media, CancellationToken cancellationToken = default)
    {
        await _botClient.SendMediaGroupAsync(userId, media, replyToMessageId: messageId, cancellationToken: cancellationToken);
    }


    public async Task AnswerCallbackQueryAsync(string callbackQueryId, CancellationToken cancellationToken = default)
    {
        await _botClient.AnswerCallbackQueryAsync(callbackQueryId, cancellationToken: cancellationToken);
    }
    public async Task AnswerCallbackQueryAsync(string callbackQueryId, string text, CancellationToken cancellationToken = default)
    {
        await _botClient.AnswerCallbackQueryAsync(callbackQueryId, text, showAlert: false, cacheTime: 0, cancellationToken: cancellationToken);
    }
}
