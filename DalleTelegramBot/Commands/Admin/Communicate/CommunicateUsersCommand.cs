using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Data.Models;
using DalleTelegramBot.Services.Telegram;
using Serilog;
using Telegram.Bot.Exceptions;
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
            var users = await _userRepository.GetAllAsync(isBan: false);
            await _telegramService.SendMessageAsync(userId, string.Format(TextUtility.CommunicateCommandStartForwardingMessageFormat, users.Count()).EscapeMarkdownV2(), ParseMode.MarkdownV2, cancellationToken);
            int sent = 0, failed = 0;
            int batchSize = 30; // send messages to 30 users at a time
            int delaySeconds = 2; // wait 2 seconds between batches

            for (int i = 0; i < users.Count(); i += batchSize)
            {
                var batch = users.Skip(i).Take(batchSize).ToList();
                var tasks = batch.Select(user => _telegramService.ForwardMessageAsync(user.Id, userId, message.MessageId, cancellationToken)).ToList();

                try
                {
                    await Task.WhenAll(tasks);
                    sent += tasks.Count;
                }
                catch (AggregateException ex)
                {
                    Log.Error(ex.Flatten(), "EX in Froward to all users");

                    foreach (var innerEx in ex.InnerExceptions)
                    {
                        if (innerEx is ApiRequestException apiEx && apiEx.ErrorCode == 429)
                        {
                            int retryAfterSeconds = apiEx.Parameters != null && apiEx.Parameters.RetryAfter != null ? apiEx.Parameters.RetryAfter.Value : delaySeconds;
                            await Task.Delay(retryAfterSeconds * 1000, cancellationToken);
                        }
                        else
                        {
                            failed++;
                        }
                    }
                }

                await Task.Delay(delaySeconds * 1000, cancellationToken);
            }


            await _telegramService.SendMessageAsync(userId, string.Format(TextUtility.CommunicateCommandEndForwardMessageFormat, sent, failed).EscapeMarkdownV2(), ParseMode.MarkdownV2, cancellationToken);
        }
    }
}
