using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Commands.Base;

internal abstract class BaseCommand : ICommand
{
    protected readonly ITelegramService _telegramService;
    public BaseCommand(ITelegramService telegramService) =>
        _telegramService = telegramService;

    public abstract Task ExecuteAsync(Message message, CancellationToken cancellationToken);
}
