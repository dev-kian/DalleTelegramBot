using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.Admin.Config;

[Query("bot-config-rate-limit")]
internal class BotRateLimitingQuery : BaseQuery, ISingletonDependency
{
    private readonly IConfiguration _configuration;
    public BotRateLimitingQuery(ITelegramService telegramService, IConfiguration configuration) : base(telegramService)
    {
        _configuration = configuration;
    }

    public override Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
