using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Configurations;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Queries.User.Account;

[Query("back-account")]
[CheckBanUser]
internal class BackAccountQuery : BaseQuery, IScopedDependency
{
    private readonly IUserRepository _userRepository;
    private readonly RateLimitingMemoryCache _cache;
    public BackAccountQuery(ITelegramService telegramService, IUserRepository userRepository, IOptionsMonitor<AppSettings> options, RateLimitingMemoryCache cache) : base(telegramService)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        long userId = callbackQuery.UserId();

        var user = await _userRepository.GetByIdAsync(userId);

        await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, TextUtility.AccountInfo(callbackQuery.From!.FirstName, userId,
            user.CreateTime, _cache.GetMessageCount(userId), BotConfig.RateLimitCount),
        InlineUtility.AccountSettingsInlineKeyboard, ParseMode.Html, cancellationToken);
    }
}