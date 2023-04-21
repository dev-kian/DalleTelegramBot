using Telegram.Bot.Types;

namespace DalleTelegramBot.Commands.Base;

internal interface ICommand
{
    Task ExecuteAsync(Message message, CancellationToken cancellationToken);
}
