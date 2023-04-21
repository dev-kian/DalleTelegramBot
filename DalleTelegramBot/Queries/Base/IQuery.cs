using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.Base;

internal interface IQuery
{
    Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
}
