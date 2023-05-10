using Telegram.Bot.Types;
using DalleTelegramBot.Services.Telegram;

namespace DalleTelegramBot.Filters.Base;

internal abstract class BaseFilter : IFilter
{
    protected readonly ITelegramService _telegramService;
    public BaseFilter(ITelegramService telegramService) =>
        _telegramService = telegramService;

    public abstract Task<bool> CheckAsync(Message message, CancellationToken cancellationToken = default);
    public abstract Task<bool> CheckAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken = default);
}
