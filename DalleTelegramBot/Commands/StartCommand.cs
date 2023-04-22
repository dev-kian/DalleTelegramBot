using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands
{
    [Command("/start", Role.Optional)]
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
                await _telegramService.SendMessageAsync(userId, TextUtility.StartInfo(userId, message.From!.FirstName, newUser: false), InlineUtility.StartCommandReplyKeyboard, ParseMode.Html, cancellationToken);
            }
            else
            {
                await _userRepository.AddAsync(new() { Id = userId });
                await _telegramService.SendMessageAsync(userId, TextUtility.StartInfo(userId, message.From!.FirstName, newUser: true), InlineUtility.StartCommandReplyKeyboard, ParseMode.Html, cancellationToken);
            }
        }
    }
}
