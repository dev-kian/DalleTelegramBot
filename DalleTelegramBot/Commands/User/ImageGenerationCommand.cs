using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Services.OpenAI;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.User
{
    [Command("create-image", Role.User)]
    [CheckBanUser]
    internal class ImageGenerationCommand : BaseCommand, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        private readonly IOpenAIClient _openAIClient;
        private readonly RateLimitingMemoryCache _rateLimitingCache;
        private readonly StateManagementMemoryCache _stateCache;
        public ImageGenerationCommand(ITelegramService telegramService, IUserRepository userRepository, IOpenAIClient openAIClient, RateLimitingMemoryCache rateLimitingCache, StateManagementMemoryCache stateCache) : base(telegramService)
        {
            _userRepository = userRepository;
            _openAIClient = openAIClient;
            _rateLimitingCache = rateLimitingCache;
            _stateCache = stateCache;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            var user = await _userRepository.GetByIdAsync(userId);

            var apiKey = user.ApiKey;

            if (message.Text.GetCommand("create-image"))
            {
                if (string.IsNullOrEmpty(apiKey))
                {
                    if (!_rateLimitingCache.IsMessageLimitExceeded(userId))
                    {
                        _stateCache.SetLastCommand(userId, "create-image", 2, data: "with-limit");
                        await _telegramService.SendMessageAsync(userId, "Send prompt to generate image", cancellationToken);
                    }
                    else
                    {
                        await _telegramService.SendMessageAsync(userId, "You can't generate image in 24 hours ago", cancellationToken);
                    }
                }
                else
                {
                    if (await _openAIClient.ValidateApiKey(apiKey, cancellationToken))
                    {
                        _stateCache.SetLastCommand(userId, "create-image", 2, data: "without-limit");
                        await _telegramService.SendMessageAsync(userId, "Send prompt to generate image", cancellationToken);
                    }
                    else
                    {
                        await _telegramService.SendMessageAsync(userId, "Your api key is not valid", cancellationToken);
                    }
                }
            }
            else if (_stateCache.CanGetLastCommand(userId, "create-image", 2, false))
            {
                var messageResponse = await _telegramService.SendMessageAsync(userId, "⏳Processing", cancellationToken);

                var imageResponse = await _openAIClient.GenerateImageAsync(new(message.Text, user.ImageCount, user.ImageSize), cancellationToken, apiKey: user.ApiKey);

                if (imageResponse.IsSuccess)
                {
                    await _telegramService.EditMessageAsync(userId, messageResponse.MessageId, $"⌛️Completed✅\n({imageResponse.Images!.ProcessingTime})", cancellationToken);

                    await _telegramService.SendChatActionAsync(userId, ChatAction.UploadPhoto, cancellationToken);

                    var media = imageResponse.Images.Data.Select(x => new InputMediaPhoto(new InputFileUrl(x.Url))).OfType<IAlbumInputMedia>();

                    await _telegramService.SendMediaGroupAsync(userId, media, cancellationToken);

                    bool hasLimit = _stateCache.CanGetCommandData(userId, true)[0].Equals("with-limit");
                    if (hasLimit)//bug if multi request can dor mizane ino
                    {
                        _rateLimitingCache.UpdateUserMessageCount(userId, imageResponse.Images.Data.Count);
                    }
                }
                else
                {
                    await _telegramService.EditMessageAsync(userId, messageResponse.MessageId,
                        $"️ Uncompleted📛\n({imageResponse.Error.ProcessingTime})\nError:{imageResponse.Error.Error.Message}");
                }
            }
        }
    }
}
