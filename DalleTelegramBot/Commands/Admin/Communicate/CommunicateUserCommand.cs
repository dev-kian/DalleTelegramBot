using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.Admin.Communicate;

[Command("communicate-user", Role.Admin)]
internal class CommunicateUserCommand : BaseCommand, IScopedDependency
{
    private readonly IUserRepository _userRepository;
    private readonly StateManagementMemoryCache _cache;
    public CommunicateUserCommand(ITelegramService telegramService, IUserRepository userRepository, StateManagementMemoryCache cache) : base(telegramService)
    {
        _userRepository = userRepository;
        _cache = cache;
    }

    public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
    {
        long userId = message.UserId();
        if (_cache.CanGetLastCommand(userId, "communicate-user", 1, false))
        {
            if (long.TryParse(message.Text, out long inputUserId))
            {
                if (await _userRepository.AnyAsync(inputUserId))
                {
                    _cache.SetLastCommand(userId, "communicate-user", 2, data: inputUserId);
                    await _telegramService.SendMessageAsync(userId, TextUtility.CommunicateCommandSendMessage, cancellationToken);
                }
                else
                {
                    await _telegramService.SendMessageAsync(userId, string.Format(TextUtility.GetUserCommandNotFoundUserIdFormat, inputUserId), cancellationToken);
                }
            }
            else
            {
                await _telegramService.SendMessageAsync(userId, TextUtility.GetUserCommandNotValidUserId, cancellationToken);
            }
        }
        else if (_cache.CanGetLastCommand(userId, "communicate-user", 2))
        {
            var fetchUserId = Convert.ToInt64(_cache.CanGetCommandData(userId, true)[0]);
            try
            {
                var messageResponse = await _telegramService.ForwardMessageAsync(fetchUserId, userId, message.MessageId, cancellationToken);
                var userName = messageResponse?.Chat?.Username;
                userName = string.IsNullOrEmpty(userName) ? "*Empty*" : $"@{userName}";
                var messageToSend = string.Format(TextUtility.CommunicateCommandMessageFormat, fetchUserId, userName).EscapeMarkdownV2();
                await _telegramService.SendMessageAsync(userId, messageToSend, ParseMode.MarkdownV2, cancellationToken);
            }
            catch (Exception ex)
            {
                await _telegramService.SendMessageAsync(userId, ex.Message, ParseMode.MarkdownV2, cancellationToken);
            }
        }
    }
}
