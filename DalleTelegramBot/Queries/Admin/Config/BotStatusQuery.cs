using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Configurations;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Queries.Admin.Config;

[Query("bot-config-bot-status")]
internal class BotStatusQuery : BaseQuery, ISingletonDependency
{
    public BotStatusQuery(ITelegramService telegramService) : base(telegramService)
    {
    }

    public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        long userId = callbackQuery.UserId();
        
        var args = callbackQuery.Data!.GetArgs();
        if(args.Any())
        {
            if (args[0] == "on")
                BotConfig.BotStatus = true;
            else if (args[0] == "off")
                BotConfig.BotStatus = false;
        }

        bool botStatus = BotConfig.BotStatus;

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            InlineUtility.KeyboardButton($"Turn {(botStatus ? "Off" : "On")}", $"bot-config-bot-status {(botStatus ? "off" : "on")}"),
            InlineUtility.BackKeyboardButton("settings"),
        });
        string message = $"Bot Status: {(botStatus ? "*ON*✅" : "*OFF*❎")}";
        await _telegramService.EditMessageAsync(userId, callbackQuery.Message!.MessageId, message, inlineKeyboard, ParseMode.MarkdownV2, cancellationToken);
    }
}
