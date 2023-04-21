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
    internal class CheckBanUserFilter : BaseFilter, IScopedDependency//TODO: maybe better use singleton
    {
        private readonly TelegramSettings _settings;
        private IUserRepository _userRepository;
        public CheckBanUserFilter(ITelegramService telegramService, IOptionsMonitor<AppSettings> options, IUserRepository userRepository) : base(telegramService)
        {
            _settings = options.CurrentValue.TelegramSettings;
            _userRepository = userRepository;
        }

        public override async Task<bool> CheckAsync(Message message, CancellationToken token = default)
        {
            var userId = message.Chat.Id;
            if (userId == _settings.AdminId)
                return true;

            var user = await _userRepository.GetByIdAsync(userId);
            if(user is null)
            {
                if (!message.Text!.GetCommand("/start"))
                {
                    await _telegramService.SendMessageAsync(userId, "Hi There!\nBefore use this but we need you send /start to register you!");
                    return false;
                }
                return true;
            }
            if (user.IsBan)
                await _telegramService.SendMessageAsync(userId, "Your account is banned!\nPlease contact support.");

            return !user.IsBan;
        }
    }
}
