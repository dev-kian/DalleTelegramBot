using Telegram.Bot.Types;

namespace DalleTelegramBot.Filters.Base;

internal interface IFilter
{
    Task<bool> CheckAsync(Message message, CancellationToken token = default);
}
