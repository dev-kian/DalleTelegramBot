using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Services.Telegram;
using DalleTelegramBot.Common.Enums;

namespace DalleTelegramBot.Commands.Admin.Account
{
    [Command("search-user", Role.Admin)]
    internal class GetUserCommand : BaseCommand, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        public GetUserCommand(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            if (!long.TryParse(message.Text, out long userIdInput))
            {
                await _telegramService.SendMessageAsync(userId, TextUtility.GetUserCommandNotValidUserId, cancellationToken);
            }
            else
            {
                var user = await _userRepository.GetByIdAsync(userIdInput);

                if (user is null)
                {
                    await _telegramService.SendMessageAsync(userId, string.Format(TextUtility.GetUserCommandNotFoundUserIdFormat, userIdInput), cancellationToken);
                }
                else
                {

                    await _telegramService.SendMessageAsync(userId,
                        TextUtility.UserInfo(user.Id, user.IsBan, user.CreateTime),
                        InlineUtility.AdminSettingsBanUserInlineKeyboard(user.Id, user.IsBan, hasBackButton: false), ParseMode.Html, cancellationToken);
                }
            }
        }
    }
}
