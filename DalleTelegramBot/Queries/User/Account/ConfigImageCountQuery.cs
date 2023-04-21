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

            var args = callbackQuery.Data!.GetArgs();
            if(args.Any() && int.TryParse(args[0], out int imgCountSelected))
            {
                await _userRepository.UpdateImageCountAsync(userId, imgCountSelected);
                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Select Image Count",
                    InlineUtility.AccountSettingsImageCountInlineKeyboard(imgCountSelected), cancellationToken);
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user is null)
                    return;

                int imgCount = user.ImageCount;
                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Select Image Count",
                    InlineUtility.AccountSettingsImageCountInlineKeyboard(imgCount), cancellationToken);
            }
        }
    }
}
