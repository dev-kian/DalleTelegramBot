using DalleTelegramBot.Commands;
using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Configurations;
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
        public TelegramUpdateHandler(CommandCollection commandCollection, QueryCollection queryCollection, StateManagementMemoryCache cache,
            IServiceProvider serviceProvider, ITelegramService telegramService, IOptionsMonitor<AppSettings> options, ILogger<TelegramUpdateHandler> logger)
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
            var handler = update switch
            {
                { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
                { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
                _ => throw new NotImplementedException()
            };

            await handler;
        }

        private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Receive message type: {MessageType}", message.Type);
            
            string command = message.Text!;

            _=InvokeCommand(command).ConfigureAwait(false);

            
            async Task InvokeCommand(string commandName)
            {
                long userId = message.UserId();

                if(!BotConfig.BotStatus && _settings.AdminId != userId)
                {
                    await _telegramService.SendMessageAsync(userId, TextUtility.BotOffMessage, cancellationToken);
                    return;
                }
                if (string.IsNullOrEmpty(commandName))
                {
                    await _telegramService.SendMessageAsync(userId, TextUtility.UnknownCommandMessage, cancellationToken);
                    return;
                }

                commandName = commandName.GetCommand().RemoveIcons();

                if(!_commandCollection.TryGetCommand(commandName, out var commandInfo))
                {
                    if(_cache.CanGetLastCommand(userId, out var lastCommand))
                        _commandCollection.TryGetCommand(lastCommand, out commandInfo);
                }
                else
                {
                    _cache.RemoveLastCommand(userId);
                }

                if(commandInfo is null)
                {
                    if(_settings.AdminId == userId)
                    {
                        await _telegramService.SendMessageAsync(userId, TextUtility.NotExistsCommandAdminMessage, cancellationToken);
                    }
                    else
                    {
                        await _telegramService.SendMessageAsync(userId, TextUtility.NotExistsCommandMessage, InlineUtility.StartCommandReplyKeyboard, cancellationToken);
                    }

                    return;
                }

                switch (commandInfo.Role)
                {
                    case Role.Admin when _settings.AdminId != userId:
                        await _telegramService.SendMessageAsync(userId, TextUtility.NotExistsCommandMessage, InlineUtility.StartCommandReplyKeyboard, cancellationToken);
                        return;

                    case Role.User when _settings.AdminId == userId:
                        await _telegramService.SendMessageAsync(userId, TextUtility.ExecuteByUserOnlyMessage, cancellationToken);
                        return;
                }

                using var scope = _serviceProvider.CreateAsyncScope();

                if (!await FilterPipeline(scope, commandInfo.TypeModel, message, cancellationToken))
                    return;

                var service = scope.ServiceProvider.GetService(commandInfo.TypeModel) as ICommand;

                if (service is null)
                {
                    throw new NullReferenceException($"Failed to get service for command '{commandName}'.");
                }

                try
                {
                    await service.ExecuteAsync(message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Execute Command EX");
                    await _telegramService.SendMessageAsync(userId, TextUtility.ExceptionHappenedMessage, cancellationToken);
                }
            }
        }

        private async Task<bool> FilterPipeline(AsyncServiceScope scope, Type type, object obj, CancellationToken cancellationToken)
        {
            var attributeTypes = type.GetCustomAttributes<FilterAttribute>();
            
            foreach (var attributeType in attributeTypes)
            {
                var filterType = attributeType.FilterType;

                if (!typeof(IFilter).IsAssignableFrom(filterType))
                    throw new ArgumentException($"{filterType.FullName} does not implement IFilter.");

                var filter = scope.ServiceProvider.GetService(filterType) as IFilter;

                if (filter is null)
                {
                    throw new NullReferenceException($"Failed to get filter service for type '{filterType.FullName}'.");
                }

                try
                {
                    var checkResult = obj switch
                    {
                        Message msg => await filter.CheckAsync(msg, cancellationToken),
                        CallbackQuery callback => await filter.CheckAsync(callback, cancellationToken),
                        _ => false
                    };

                    if (!checkResult)
                        return false;
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred while executing a filter of type '{filterType.FullName}'.", ex);
                }
            }

            return true;
        }

        private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            // If the callback query message or data text is null, return without doing anything.
            if (callbackQuery.Message is null || callbackQuery.Data is not { } dataText)
                return;

            long userId = callbackQuery.UserId();
            if(!BotConfig.BotStatus && _settings.AdminId != userId)
            {
                await _telegramService.AnswerCallbackQueryAsync(callbackQuery.Id, TextUtility.BotOffMessage, cancellationToken);
                return;
            }

            // Extract the query name from the data text and invoke the corresponding query.
            var query = dataText.GetQuery();
            _=InvokeCallbackQuery(query);

            // Executes the specified query by name, if it exists.
            async Task InvokeCallbackQuery(string queryName)
            {
                // Try to get the query type by name from the query collection.
                if (_queryCollection.TryGetQuery(queryName, out Type? queryType))
                {
                    // If the query type is null, throw an exception.
                    if (queryType is null)
                        throw new NullReferenceException($"Query type for '{queryName}' is null.");

                    _cache.RemoveLastCommand(userId);

                    // Create an async service scope to resolve dependencies for the query.
                    using var scope = _serviceProvider.CreateAsyncScope();


                    if (!await FilterPipeline(scope, queryType, callbackQuery, cancellationToken))
                        return;

                    // Get the query object from the service provider.
                    var query = scope.ServiceProvider.GetService(queryType) as IQuery;

                    // If the query object is null, throw an exception.
                    if (query is null)
                        throw new NullReferenceException($"Failed to get query for '{queryName}'.");

                    try
                    {
                        // Execute the query asynchronously, passing in the callback query and cancellation token.
                        await query!.ExecuteAsync(callbackQuery, cancellationToken);

                        await _telegramService.AnswerCallbackQueryAsync(callbackQuery.Id, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Execute Query EX");
                        await _telegramService.AnswerCallbackQueryAsync(callbackQuery.Id, TextUtility.ExceptionHappenedMessage, cancellationToken);
                    }
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

            _logger.LogError("HandleError: {ErrorMessage}", errorMessage);

            // Add a cooldown period in case of network connection errors to avoid overwhelming the API with requests
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
        }
    }
}
