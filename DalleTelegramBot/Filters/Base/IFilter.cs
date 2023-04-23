using Telegram.Bot.Types;

namespace DalleTelegramBot.Filters.Base;

internal interface IFilter
{
    Task<bool> CheckAsync(Message message, CancellationToken cancellationToken = default);
    Task<bool> CheckAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken = default);
}
