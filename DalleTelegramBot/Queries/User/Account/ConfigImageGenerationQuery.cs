using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Queries.User.Account
{
    [Query("config-img-generation")]
    [CheckBanUser]
    internal class ConfigImageGenerationQuery : BaseQuery, IScopedDependency
    {
        public ConfigImageGenerationQuery(ITelegramService telegramService) : base(telegramService)
        {
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineUtility.KeyboardButton("Image Size", "config-img-size"),
                InlineUtility.KeyboardButton("Count Generate", "config-img-count"),
                InlineUtility.BackKeyboardButton("account"),
            });

            await _telegramService.EditMessageAsync(callbackQuery.UserId(), callbackQuery.Message!.MessageId,
                "Config Image", inlineKeyboard, cancellationToken);
        }
    }
}
