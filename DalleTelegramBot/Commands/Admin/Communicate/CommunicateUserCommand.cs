using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DalleTelegramBot.Commands.Admin.Communicate
{
    [Command("communicate-user", adminRequired: true)]
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
                        await _telegramService.SendMessageAsync(userId, "Send your message", cancellationToken);
                    }
                    else
                    {
                        await _telegramService.SendMessageAsync(userId, string.Format(TextConstant.GetUserCommandNotFoundUserIdFormat, inputUserId), cancellationToken);
                    }
                }
                else
                {
                    await _telegramService.SendMessageAsync(userId, string.Format(TextConstant.GetUserCommandNotValidUserId, inputUserId), cancellationToken);
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
                    var messageToSend = $"Your message has been successfully forwarded to {fetchUserId} ({userName})".EscapeMarkdownV2();
                    await _telegramService.SendMessageAsync(userId, messageToSend, ParseMode.MarkdownV2, cancellationToken);
                }
                catch (Exception ex)
                {
                    await _telegramService.SendMessageAsync(userId, ex.Message, ParseMode.MarkdownV2, cancellationToken);
                }
            }
        }
    }

    [Command("communicate-users", adminRequired: true)]
    internal class CommunicateUsers : BaseCommand, IScopedDependency
    {
        private readonly IUserRepository _userRepository;
        private readonly StateManagementMemoryCache _cache;
        public CommunicateUsers(ITelegramService telegramService, IUserRepository userRepository, StateManagementMemoryCache cache) : base(telegramService)
        {
            _userRepository = userRepository;
            _cache = cache;
        }

        public override async Task ExecuteAsync(Message message, CancellationToken cancellationToken)
        {
            long userId = message.UserId();

            if(_cache.CanGetLastCommand(userId, "communicate-users", 1, true))
            {
                var usersBanned = await _userRepository.GetAllAsync(isBan: false);
                await _telegramService.SendMessageAsync(userId, $"Start forwarding your message to *{usersBanned.Count()}* users", ParseMode.MarkdownV2, cancellationToken);
                var usersChunks = usersBanned.Chunk(5);//todo: after change to 100
                int s = 0, f = 0;
                foreach (var usersChunk in usersChunks)
                {
                    var tasks = usersChunk.Select(x => Task.Run(async () =>
                    {
                        try
                        {
                            await _telegramService.ForwardMessageAsync(x.Id, userId, message.MessageId, cancellationToken);
                            Interlocked.Increment(ref s);
                        }
                        catch
                        {
                            Interlocked.Increment(ref f);
                        }
                    }));
                    await Task.WhenAll(tasks);
                }

                await _telegramService.SendMessageAsync(userId, $"Your message has been successfully forwarded to *{s}* users\nFailed can't send: *{f}*", ParseMode.MarkdownV2, cancellationToken);
            }
        }
    }
}
