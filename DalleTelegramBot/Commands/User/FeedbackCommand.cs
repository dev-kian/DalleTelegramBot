using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Services.Telegram;
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
                $"Dear {message.From!.FirstName}, to contact the robot _support_, you can refer to the Telegram ID below\n@jkianj", ParseMode.Markdown, cancellationToken);

            await _telegramService.SendMessageAsync(userId,
                $"Dear {message.From!.FirstName}, to contact the robot _support_, you can refer to the Telegram ID below\n@jkianj", ParseMode.MarkdownV2, cancellationToken);
        }
    }
}
