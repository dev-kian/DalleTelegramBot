using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.Admin.Communicate
{
    [Query("communicate-users")]
    internal class CommunicateUsersQuery : BaseQuery, ISingletonDependency
    {
        private readonly StateManagementMemoryCache _cache;
        public CommunicateUsersQuery(ITelegramService telegramService, StateManagementMemoryCache cache) : base(telegramService)
        {
            _cache = cache;
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            long userId = callbackQuery.UserId();

            _cache.SetLastCommand(userId, "communicate-users", 1);

            await _telegramService.SendMessageAsync(callbackQuery.UserId(), "Send your message to forward", cancellationToken);
        }
    }
}
