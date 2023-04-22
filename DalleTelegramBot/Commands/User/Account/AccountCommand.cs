using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Configurations;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.User.Account
{
    [Command("account⚙️")]
    internal class AccountCommand : BaseCommand, IScopedDependency
    {
        private readonly RateLimitingMemoryCache _cache;
        public AccountCommand(ITelegramService telegramService, RateLimitingMemoryCache cache) : base(telegramService)
        {
            _cache = cache;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            await _telegramService.SendMessageAsync(userId, TextUtilitiy.AccountInfo(_cache.GetMessageCount(userId), BotConfig.LimitCount),
                InlineUtility.AccountSettingsInlineKeyboard, ParseMode.Markdown, cancellationToken);
        }
    }
}
