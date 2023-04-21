using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.User.Account
{
    [Command("/account")]
    internal class AccountCommand : BaseCommand, IScopedDependency
    {
        public AccountCommand(ITelegramService telegramService) : base(telegramService)
        {
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            await _telegramService.SendMessageAsync(message.UserId(), TextConstant.AccountInfo(1, 20),
                InlineUtility.AccountSettingsInlineKeyboard, ParseMode.Markdown, cancellationToken);
        }
    }
}
