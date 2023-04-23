using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Filters.Base;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Filters
{
    internal class CheckBanUserFilter : BaseFilter, IScopedDependency
    {
        private readonly TelegramSettings _settings;
        private IUserRepository _userRepository;
        public CheckBanUserFilter(ITelegramService telegramService, IOptionsMonitor<AppSettings> options, IUserRepository userRepository) : base(telegramService)
        {
            _settings = options.CurrentValue.TelegramSettings;
            _userRepository = userRepository;
        }

        public override async Task<bool> CheckAsync(Message message, CancellationToken cancellationToken = default)
        {
            var userId = message.UserId();
            if (IsAdmin(userId))
                return true;

            var user = await _userRepository.GetByIdAsync(userId);
            if(user is null)
            {
                if (!message.Text!.GetCommand("/start"))
                {
                    await _telegramService.SendMessageAsync(userId, "Unfortunately, we could not find your account, please send the /start command to recreate the account");
                    return false;
                }
                return true;
            }
            if (user.IsBan)
                await _telegramService.SendMessageAsync(userId, "Your account is banned!\nPlease contact support.");

            return !user.IsBan;
        }

        public override async Task<bool> CheckAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken = default)
        {
            var userId = callbackQuery.UserId();
            if (IsAdmin(userId))
                return true;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                await _telegramService.AnswerCallbackQueryAsync(callbackQuery.Id, "Unfortunately, we could not find your account, please send the /start command to recreate the account");
                return false;
            }
            if (user.IsBan)
                await _telegramService.AnswerCallbackQueryAsync(callbackQuery.Id, "Your account is banned!", cancellationToken);

            return !user.IsBan;
        }

        private bool IsAdmin(long userId)
            => userId == _settings.AdminId;
    }
}
