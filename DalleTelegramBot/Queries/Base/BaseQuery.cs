using Telegram.Bot.Types;
using DalleTelegramBot.Services.Telegram;

namespace DalleTelegramBot.Queries.Base;

internal abstract class BaseQuery : IQuery
{
    protected readonly ITelegramService _telegramService;
    public BaseQuery(ITelegramService telegramService) =>
        _telegramService = telegramService;

    public abstract Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken);
}
