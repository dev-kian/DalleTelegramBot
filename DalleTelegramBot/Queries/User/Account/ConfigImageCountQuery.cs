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
    [Query("config-img-count")]
    [CheckBanUser]
    internal class ConfigImageCountQuery : BaseQuery, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        public ConfigImageCountQuery(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            long userId = callbackQuery.UserId();

            var user = await _userRepository.GetByIdAsync(userId);

            int imgCount = user.ImageCount;

            var args = callbackQuery.Data!.GetArgs();

            if(args.Any() && int.TryParse(args[0], out int imgCountSelected))
            {
                //The following condition is to handle this error:
                //[BadRequest]: message is not modified specified new message content and reply markup are exactly the same as a current content and reply markup of the message
                if (imgCountSelected == imgCount)
                    return;

                await _userRepository.UpdateImageCountAsync(userId, imgCountSelected);
                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Select Image Count",
                    InlineUtility.AccountSettingsImageCountInlineKeyboard(imgCountSelected), cancellationToken);
            }
            else
            {
                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Select Image Count",
                    InlineUtility.AccountSettingsImageCountInlineKeyboard(imgCount), cancellationToken);
            }
        }
    }
}
