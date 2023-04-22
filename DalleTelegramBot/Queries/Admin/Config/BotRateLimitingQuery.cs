using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.Admin.Config;

[Query("bot-config-rate-limit")]
internal class BotRateLimitingQuery : BaseQuery, ISingletonDependency
{
    public BotRateLimitingQuery(ITelegramService telegramService) : base(telegramService)
    {
    }

    public override Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
