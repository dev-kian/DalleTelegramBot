using DalleTelegramBot.Common;
using DalleTelegramBot.Data.Repositories;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DalleTelegramBot
{
    internal class HostedService : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly AppSettings _settings;
        private ITelegramService _telegramService;
        private readonly ILogger<HostedService> _logger;
        public HostedService(IConfiguration configuration, IServiceProvider serviceProvider, 
            IOptionsMonitor<AppSettings> options, ITelegramService telegramService, ILogger<HostedService> logger)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            _telegramService = telegramService;
            _settings = options.CurrentValue;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //Initializes the SQLite database using the configuration settings.
            _logger.LogInformation("Initializes database...");
            await DatabaseInitializer.InitializeAsync(_configuration);

            // Start the polling service
            _logger.LogInformation("Starting polling service");

            await _telegramService.SendMessageAsync(_settings.TelegramSettings.AdminId, "Bot turned on...", stoppingToken);

            _= StartReceivingAsync(stoppingToken);
        }
        
        // Stop the polling service
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        // Continuously receive messages until the cancellation token is triggered
        private async Task StartReceivingAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // Create a new scope to resolve dependencies
                    using var scope = _serviceProvider.CreateAsyncScope();
                    var receiverService = scope.ServiceProvider.GetRequiredService<ReceiverService>();

                    // Receive a message and process it asynchronously
                    await receiverService.ReceiveAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log any exceptions that occur during polling and wait for a short period of time
                    _logger.LogError("Polling failed with exception: {Exception}", ex);
                    await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                }
            }
        }
    }
}
