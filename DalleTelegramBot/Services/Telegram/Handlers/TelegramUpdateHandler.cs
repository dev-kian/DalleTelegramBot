using DalleTelegramBot.Commands;
using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Filters.Base;
using DalleTelegramBot.Queries;
using DalleTelegramBot.Queries.Base;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Services.Telegram.Handlers
{
    internal class TelegramUpdateHandler : IUpdateHandler
    {
        private readonly CommandCollection _commandCollection;
        private readonly QueryCollection _queryCollection;
        private readonly StateManagementMemoryCache _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITelegramService _telegramService;
        private TelegramSettings _settings;
        private readonly ILogger<TelegramUpdateHandler> _logger;
        public TelegramUpdateHandler(CommandCollection commandCollection, QueryCollection queryCollection,
            StateManagementMemoryCache cache, IServiceProvider serviceProvider, ITelegramService telegramService, IOptionsMonitor<AppSettings> options, ILogger<TelegramUpdateHandler> logger)
        {
            _commandCollection = commandCollection;
            _queryCollection = queryCollection;
            _cache = cache;
            _serviceProvider = serviceProvider;
            _telegramService = telegramService;
            _settings = options.CurrentValue.TelegramSettings;
            _logger = logger;
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            try
            {
                var handler = update switch
                {
                    { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                    { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                    _ => throw new NotImplementedException()
                };

                await handler;
            }
            catch (Exception ex)
            {
                _logger.LogError("exception", ex);
            }

        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Receive message type: {MessageType}", message.Type);
            //if (message.Text is not { } messageText)
            //    return;

            string command = (message.Text ?? "").GetCommand();

            _= InvokeCommand(command).ConfigureAwait(false);
            
            async Task InvokeCommand(string commandName)
            {
                if (!string.IsNullOrEmpty(command) && _commandCollection.TryGetCommand(commandName, out var typeInfo))
                {
                    var scope = _serviceProvider.CreateAsyncScope();

                    if (typeInfo.AdminRequired)
                    {
                        if (_settings.AdminId == message.UserId())
                        {
                            var service = scope.ServiceProvider.GetService(typeInfo.TypeModel) as ICommand;
                            await service.ExecuteAsync(message, cancellationToken);
                        }
                        else
                        {
                            await _telegramService.SendMessageAsync(message.UserId(), "Get help");
                        }
                    }
                    else
                    {
                        if (!await FilterPipeline(scope, typeInfo.TypeModel, message, cancellationToken))
                            return;

                        var service = scope.ServiceProvider.GetService(typeInfo.TypeModel) as ICommand;
                        await service.ExecuteAsync(message, cancellationToken);
                    }
                }
                else if (_cache.CanGetLastCommand(message.UserId(), out string lastCommand))
                {
                    if(_commandCollection.TryGetCommand(lastCommand, out var cInfo))
                    {
                        var scope = _serviceProvider.CreateAsyncScope();

                        if (cInfo.AdminRequired)
                        {
                            if (_settings.AdminId == message.UserId())
                            {
                                var service = scope.ServiceProvider.GetService(cInfo.TypeModel) as ICommand;
                                await service.ExecuteAsync(message, cancellationToken);
                            }
                            else
                            {
                                await _telegramService.SendMessageAsync(message.UserId(), "Get help");
                            }
                        }
                        else
                        {
                            if (!await FilterPipeline(scope, cInfo.TypeModel, message, cancellationToken))
                                return;

                            var service = scope.ServiceProvider.GetService(cInfo.TypeModel) as ICommand;
                            await service.ExecuteAsync(message, cancellationToken);
                        }
                    }
                }
                else
                {
                    await _telegramService.SendMessageAsync(message.UserId(), "I can't understand what do you need?");
                }
            }
        }
        async Task<bool> FilterPipeline(AsyncServiceScope scope, Type commandType, Message message, CancellationToken cancellationToken)
        {
            var types = commandType.GetCustomAttributes<FilterAttribute>();
            foreach (var attributeType in types)
            {
                var filterType = attributeType.FilterType;
                var filterService = scope.ServiceProvider.GetService(filterType) as IFilter;
                if (!await filterService?.CheckAsync(message, cancellationToken))
                {
                    return false;
                }
            }
            return true;
        }


        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery.Message is null || callbackQuery.Data is not { } dataText)
                return;

            var query = dataText.GetQuery();
            _= InvokeCallbackQuery(query);

            async Task InvokeCallbackQuery(string queryName)
            {
                if(_queryCollection.TryGetQuery(queryName, out Type? queryType))
                {
                    if (queryType is null)
                        throw new NullReferenceException();//TODO: write text for error

                    using var scope = _serviceProvider.CreateAsyncScope();

                    var query = scope.ServiceProvider.GetRequiredService(queryType) as IQuery;
                    await query!.ExecuteAsync(callbackQuery, cancellationToken);
                }
            }
        }

        public async Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // Generate an error message based on the type of exception that occurred
            var errorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()//Return the [Message] [Name of the exception class] [Source] [StackTrace] 
            };

            _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);

            // Add a cooldown period in case of network connection errors to avoid overwhelming the API with requests
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
