using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Commands
{
    [Command("/start")]
    [CheckBanUser]
    internal class StartCommand : BaseCommand, IScopedDependency
    {
        private readonly TelegramSettings _settings;
        private IUserRepository _userRepository;
        public StartCommand(ITelegramService telegramService, IOptionsMonitor<AppSettings> options, IUserRepository userRepository) : base(telegramService)
        {
            _settings = options.CurrentValue.TelegramSettings;
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            if(userId == _settings.AdminId)
            {
                await _telegramService.SendMessageAsync(userId, "Choose", InlineUtility.AdminSettingsInlineKeyboard, cancellationToken);
            }
            else if(await _userRepository.AnyAsync(userId))
            {
                await _telegramService.SendMessageAsync(userId, TextConstant.StartCommandExistsUser, InlineUtility.StartCommandReplyKeyboard, cancellationToken);
            }
            else
            {
                await _userRepository.AddAsync(new() { Id = userId });
                await _telegramService.SendMessageAsync(userId, TextConstant.StartCommandNotExistsUser, InlineUtility.StartCommandReplyKeyboard, cancellationToken);
            }
        }
    }
}
