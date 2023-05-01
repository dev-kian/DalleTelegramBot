using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot.Polling;
using Telegram.Bot;
using DalleTelegramBot.Services.Telegram.Handlers;

namespace DalleTelegramBot.Services.Telegram;

internal class ReceiverService : ReceiverService<TelegramUpdateHandler>
{
    public ReceiverService(ITelegramBotClient botClient, TelegramUpdateHandler updateHandler, ILogger<ReceiverService<TelegramUpdateHandler>> logger, IOptions<ReceiverOptions> options)
        : base(botClient, updateHandler, logger, options) { }
}

internal abstract class ReceiverService<TUpdateHandler> where TUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger<ReceiverService<TUpdateHandler>> _logger;
    private readonly ReceiverOptions receiverOptions;
    public ReceiverService(ITelegramBotClient botClient, TUpdateHandler updateHandler, ILogger<ReceiverService<TUpdateHandler>> logger, IOptions<ReceiverOptions> options)
    {
        _botClient = botClient;
        _updateHandler = updateHandler;
        _logger = logger;
        receiverOptions = options.Value;
    }
    public async Task ReceiveAsync(CancellationToken stoppingToken)
    {
        var me = await _botClient.GetMeAsync(stoppingToken);
        _logger.LogInformation("Start receiving updates for {BotName}", me.Username ?? "Empty Username");

        // Start receiving updates
        await _botClient.ReceiveAsync(
            updateHandler: _updateHandler,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken);
    }
}
