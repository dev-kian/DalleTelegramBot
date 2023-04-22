using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Extensions;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Common.Utilities;
using DalleTelegramBot.Data.Contracts;
using DalleTelegramBot.Queries.Base;
using DalleTelegramBot.Services.Telegram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Queries.Admin.Communicate
{
    [Query("communicate")]
    internal class CommunicateQuery : BaseQuery, ISingletonDependency
    {
        private readonly IUserRepository _userRepository;
        public CommunicateQuery(ITelegramService telegramService, IUserRepository userRepository) : base(telegramService)
        {
            _userRepository = userRepository;
        }

        public override async Task ExecuteAsync(CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var usersCount = await _userRepository.GetCountAsync();
            
            var usersBannedCount = await _userRepository.GetCountAsync(isBan: true);
            
            var canSendCount = usersCount - usersBannedCount;

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                InlineUtility.KeyboardButton("Send message to all users", "communicate-users"),
                InlineUtility.KeyboardButton("Send message to one user", "communicate-user"),
                InlineUtility.BackKeyboardButton("settings"),
            });

            await _telegramService.EditMessageAsync(callbackQuery.UserId(), callbackQuery.Message!.MessageId, string.Format(TextUtility.CommunicateQueryMessageFormat,
                usersCount, usersBannedCount, canSendCount), inlineKeyboard, ParseMode.Markdown, cancellationToken);
        }
    }
}
