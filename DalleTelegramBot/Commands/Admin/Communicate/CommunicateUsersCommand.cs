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

[Command("communicate-users", Role.Admin)]
internal class CommunicateUsersCommand : BaseCommand, IScopedDependency
{
    private readonly IUserRepository _userRepository;
    private readonly StateManagementMemoryCache _cache;
    public CommunicateUsersCommand(ITelegramService telegramService, IUserRepository userRepository, StateManagementMemoryCache cache) : base(telegramService)
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
            await _telegramService.SendMessageAsync(userId, TextUtility.CommunicateCommandStartForwardingMessageFormat, ParseMode.MarkdownV2, cancellationToken);
            var usersChunks = usersBanned.Chunk(100);
            int sent = 0, failed = 0;
            foreach (var usersChunk in usersChunks)
            {
                var tasks = usersChunk.Select(x => Task.Run(async () =>
                {
                    try
                    {
                        await _telegramService.ForwardMessageAsync(x.Id, userId, message.MessageId, cancellationToken);
                        Interlocked.Increment(ref sent);
                    }
                    catch
                    {
                        Interlocked.Increment(ref failed);
                    }
                }));
                await Task.WhenAll(tasks);
            }

            await _telegramService.SendMessageAsync(userId, string.Format(TextUtility.CommunicateCommandEndForwardMessageForamt, sent, failed), ParseMode.MarkdownV2, cancellationToken);
        }
    }
}
