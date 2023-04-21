using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Queries.User.Account
{
    [Query("config-img-size")]
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

            var args = callbackQuery.Data!.GetArgs();

            if(args.Length == 1 && int.TryParse(args[0], out int imgSizeSelected))
            {
                await _userRepository.UpdateImageSizeAsync(userId, imgSizeSelected);

                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Image Size",
                    InlineUtility.AccountSettingsImageSizeInlineKeyboard(imgSizeSelected), cancellationToken);
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user is null)
                    return;
                int imgSize = (int)user.ImageSize;

                await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, "Image Size",
                    InlineUtility.AccountSettingsImageSizeInlineKeyboard(imgSize), cancellationToken);
            }
        }
    }
}
