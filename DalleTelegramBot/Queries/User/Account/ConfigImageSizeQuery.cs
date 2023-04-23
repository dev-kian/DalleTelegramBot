using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.User.Account
{
    [Query("config-img-size")]
    [CheckBanUser]
    internal class ConfigImageSizeQuery : BaseQuery, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        public ConfigImageSizeQuery(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            long userId = callbackQuery.UserId();

            var user = await _userRepository.GetByIdAsync(userId);

            int imgSize = (int)user.ImageSize;

            var args = callbackQuery.Data!.GetArgs();

            if (args.Length == 1 && int.TryParse(args[0], out int imgSizeSelected))
            {
                //The following condition is to handle this error:
                //[BadRequest]: message is not modified specified new message content and reply markup are exactly the same as a current content and reply markup of the message
                if (imgSizeSelected == imgSize)
                    return;

                await _userRepository.UpdateImageSizeAsync(userId, imgSizeSelected);

                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Image Size",
                    InlineUtility.AccountSettingsImageSizeInlineKeyboard(imgSizeSelected), cancellationToken);
            }
            else
            {
                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Image Size",
                    InlineUtility.AccountSettingsImageSizeInlineKeyboard(imgSize), cancellationToken);
            }
        }
    }
}
