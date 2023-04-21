using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Commands.User
{
    [Command("/feedback")]
    internal class FeedbackCommand : BaseCommand, ISingletonDependency
    {
        public FeedbackCommand(ITelegramService telegramService) : base(telegramService)
        {
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            await _telegramService.SendMessageAsync(userId, "You can talk with @jkianj", cancellationToken);
        }
    }
}
