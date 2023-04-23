using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Queries.User.Account;

[Query("config-api-key")]
[CheckBanUser]
internal class ConfigApiKeyQuery : BaseQuery, IScopedDependency
{
    private readonly IUserRepository _userRepository;
    private readonly StateManagementMemoryCache _cache;
    public ConfigApiKeyQuery(ITelegramService telegramService, IUserRepository userRepository, StateManagementMemoryCache cache) : base(telegramService)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        long userId = callbackQuery.UserId();

        var args = callbackQuery.Data!.GetArgs();
        if (args.Any())
        {
            if (args[0] == "remove")
            {
                await _userRepository.UpdateApiKeyAsync(userId, apiKey: null);
            }
            else if (args[0] == "set")
            {
                _cache.SetLastCommand(userId, "config-api-key", 1, data: callbackQuery.Message!.MessageId);
                await _telegramService.SendMessageAsync(userId, TextUtility.ConfigApiKeyGiveSecKey, cancellationToken);
                return;
            }
        }

        var user = await _userRepository.GetByIdAsync(userId);

        bool hasApiKey = !string.IsNullOrEmpty(user.ApiKey);

        var message = hasApiKey ? string.Format(TextUtility.ConfigApiKeyHasValueFormat, user.ApiKey.MaskApiKey()) : TextUtility.ConfigApiKeyHasNotValue;

        await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, message,
            InlineUtility.AccountSettingsApiKeyInlineKeyboard(hasApiKey), ParseMode.Markdown, cancellationToken);
    }
}