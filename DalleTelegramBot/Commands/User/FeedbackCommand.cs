using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Services.Telegram;
using System.Xml.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.User
{
    [Command("/feedback", Role.User)]
    internal class FeedbackCommand : BaseCommand, ISingletonDependency
    {
        public FeedbackCommand(ITelegramService telegramService) : base(telegramService)
        {
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();
            await _telegramService.SendMessageAsync(userId,
                $"Dear _{message.From!.FirstName}_, to contact the bot support, you can refer to the Telegram ID @jkianj", ParseMode.Markdown, cancellationToken);
        }
    }
}
