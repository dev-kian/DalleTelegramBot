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

[Query("get-user")]
internal class GetUserQuery : BaseQuery, IScopedDependency
{
    private readonly IUserRepository _userRepository;
    public GetUserQuery(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
    {
        _userRepository = userRepository;
    }

    public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        long userId = long.Parse(callbackQuery.Data!.GetArgs()[0]);

        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            return;

        await _telegramService.EditMessageAsync(callbackQuery.UserId(), callbackQuery.Message!.MessageId,
            TextUtility.UserInfo(user.Id, user.IsBan, user.CreateTime),
            InlineUtility.AdminSettingsBanUserInlineKeyboard(user.Id, user.IsBan), ParseMode.Html, cancellationToken);
    }
}
