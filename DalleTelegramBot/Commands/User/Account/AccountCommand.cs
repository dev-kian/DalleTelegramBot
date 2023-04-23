using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Configurations;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.User.Account
{
    [Command("account", Role.User)]
    [CheckBanUser]
    internal class AccountCommand : BaseCommand, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        private readonly RateLimitingMemoryCache _cache;
        public AccountCommand(ITelegramService telegramService, IUserRepository userRepository, IOptionsMonitor<AppSettings> options, RateLimitingMemoryCache cache) : base(telegramService)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            var user = await _userRepository.GetByIdAsync(userId);
            
                await _telegramService.SendMessageAsync(userId, TextUtility.AccountInfo(message.From!.FirstName, userId,
                    user.CreateTime, _cache.GetMessageCount(userId), BotConfig.RateLimitCount),
                InlineUtility.AccountSettingsInlineKeyboard, ParseMode.Html, cancellationToken);
        }
    }
}
