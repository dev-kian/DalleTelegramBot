using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Queries.Admin.Config
{
    [Query("bot-config")]
    internal class BotConfigQuery : BaseQuery, ISingletonDependency
    {
        public BotConfigQuery(ITelegramService telegramService) : base(telegramService)
        {
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineUtility.KeyboardButton("🗝 Set default api key", "bot-config-default-api-key"),
                InlineUtility.KeyboardButton("👮‍♀️ Limit image generation", "bot-config-rate-limit"),
                InlineUtility.KeyboardButton($"📊 Bot status", "bot-config-bot-status"),
                InlineUtility.KeyboardButton($"📑 Get log report", "bot-config-get-log-report"),
                InlineUtility.BackKeyboardButton("settings"),
            });
            await _telegramService.EditMessageAsync(callbackQuery.UserId(), callbackQuery.Message!.MessageId, await TextUtility.OSInfoText(), inlineKeyboard, ParseMode.Markdown, cancellationToken);
        }
    }
}
