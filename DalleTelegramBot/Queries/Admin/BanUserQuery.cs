using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Queries.Admin;

[Query("ban-user")]
internal class BanUserQuery : BaseQuery, IScopedDependency
{
    private readonly IUserRepository _userRepository;

    public BanUserQuery(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
    {
        _userRepository = userRepository;
    }

    public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        var args = callbackQuery.Data!.GetArgs();
        
        long userId = long.Parse(args[0]);
        
        bool hasBackButton = args.Length == 2 && args[1]=="x";

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            return;

        user.IsBan = !user.IsBan;

        await _userRepository.UpdateBanStateAsync(userId, user.IsBan);

        await _telegramService.EditMessageAsync(callbackQuery.UserId(), callbackQuery.Message!.MessageId,
            TextUtility.UserInfo(user.Id, user.IsBan, user.CreateTime),
            InlineUtility.AdminSettingsBanUserInlineKeyboard(user.Id, user.IsBan, hasBackButton), ParseMode.Html, cancellationToken);
    }
}
