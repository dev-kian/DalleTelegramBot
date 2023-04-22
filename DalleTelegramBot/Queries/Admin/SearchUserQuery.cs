using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Caching;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;

namespace DalleTelegramBot.Queries.Admin
{
    [Query("search-user")]
    internal class SearchUserQuery : BaseQuery, ISingletonDependency
    {
        private readonly StateManagementMemoryCache _cache;
        public SearchUserQuery(ITelegramService telegramService, StateManagementMemoryCache cache) : base(telegramService)
        {
            _cache = cache;
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            _cache.SetLastCommand(callbackQuery.UserId(), "search-user");
            await _telegramService.SendMessageAsync(callbackQuery.UserId(), TextUtility.SearchUserCommandAsk, cancellationToken);
        }
    }
}
