using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Pagination;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Data.Models;
using DalleTelegramBot.Data.Repositories;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Queries.Admin
{
    [Query("all-users")]
    internal class AllUsersQuery : BaseQuery, IScopedDependency
    {
        private const int PageSize = 10;
        
        private readonly IUserRepository _userRepository;
        public AllUsersQuery(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var args = callbackQuery.Data!.GetArgs();

            int pageIndex = int.Parse(args[0]);
            
            bool seeBannedUsers = args.Length is 2 && args[1]=="x";

            IEnumerable<Data.Models.User> users =
                await (seeBannedUsers ? _userRepository.GetAllAsync(isBan: true) : _userRepository.GetAllAsync());

            var usersPaginated = PaginatedList<Data.Models.User>.Create(users, pageIndex, PageSize);

            var inlineKeyboardButtons = new List<IEnumerable<InlineKeyboardButton>>();

            var usersChunks = usersPaginated.Chunk(2);
            
            foreach (var usersChunk in usersChunks)
            {
                var callbackQueries = usersChunk.Select(x => InlineKeyboardButton.WithCallbackData($"{x.Id}", $"get-user {x.Id}"));
                inlineKeyboardButtons.Add(callbackQueries);
            }

            var requirementButtons = new List<InlineKeyboardButton>();
            if (usersPaginated.HasPreviousPage)
                requirementButtons.Add(InlineKeyboardButton.WithCallbackData("◀️", $"all-users {usersPaginated.PageIndex-1}{(seeBannedUsers ? " x" : "")}"));
            if (usersPaginated.HasNextPage)
                requirementButtons.Add(InlineKeyboardButton.WithCallbackData("▶️", $"all-users {usersPaginated.PageIndex+1}{(seeBannedUsers ? " x" : "")}"));

            inlineKeyboardButtons.Add(requirementButtons);

            inlineKeyboardButtons.Add(InlineUtility.BackKeyboardButton("settings"));

            InlineKeyboardMarkup inlineKeyboard = new(inlineKeyboardButtons);

            await _telegramService.EditMessageAsync(callbackQuery.UserId(), callbackQuery.Message!.MessageId,
                $"Page: {usersPaginated.PageIndex+1}", inlineKeyboard, cancellationToken);
        }
    }
}
