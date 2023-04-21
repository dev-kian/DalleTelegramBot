using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.Admin
{
    [Query("back-settings")]
    internal class BackSettingsQuery : BaseQuery, IScopedDependency
    {
        public BackSettingsQuery(ITelegramService telegramService) : base(telegramService)
        {
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await _telegramService.EditMessageAsync(callbackQuery.UserId(),
                callbackQuery.Message!.MessageId, "Choose", InlineUtility.AdminSettingsInlineKeyboard, cancellationToken);
        }
    }
}
