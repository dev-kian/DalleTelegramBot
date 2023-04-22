using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Data.Models;
using DalleTelegramBot.Services.Telegram;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.User.Account
{
    [Command("config-api-key")]
    internal class ConfigApiKeyCommand : BaseCommand, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        private readonly StateManagementMemoryCache _cache;

        public ConfigApiKeyCommand(ITelegramService telegramService, IUserRepository userRepository, StateManagementMemoryCache cache) : base(telegramService)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();
            if(_cache.CanGetLastCommand(userId, "config-api-key", 1, false))
            {
                var apiKey = message.Text;

                if (!apiKey.ValidateApiKey())
                {
                    await _telegramService.SendMessageAsync(userId, "Your api key is not valid", cancellationToken);
                    return;
                }

                await _userRepository.UpdateApiKeyAsync(userId, apiKey);


                var fetchMessageId = Convert.ToInt32(_cache.CanGetCommandData(userId, true).FirstOrDefault());
                var tasks = new List<Task>();
                for (int i = fetchMessageId + 1; i <= message.MessageId; i++)
                {
                    tasks.Add(_telegramService.DeleteMessageAsync(userId, i, cancellationToken));
                }
                _=Task.WhenAll(tasks);

                var messageText = string.Format(TextUtilitiy.ConfigApiKeyHasValueFormat, apiKey.MaskApiKey());

                await _telegramService.EditMessageAsync(userId, fetchMessageId, messageText,
                    InlineUtility.AccountSettingsApiKeyInlineKeyboard(true), ParseMode.Markdown, cancellationToken);
            }
        }
    }
}
