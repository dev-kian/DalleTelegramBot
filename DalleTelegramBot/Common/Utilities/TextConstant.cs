using DalleTelegramBot.Common.SystemMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalleTelegramBot.Common.Utilities
{
    internal class TextConstant
    {
        public const string StartCommandExistsUser = "Before register";
        public const string StartCommandNotExistsUser = "New user\nRegister...";

        public const string GetUserCommandNotValidUserId = "Your input is valid!\nEnter digits user id";
        public const string GetUserCommandNotFoundUserIdFormat = "Id {0} was not found!";
        
        
        public const string SearchUserCommandAsk = "Please send user id to search";

        public const string CommunicateQueryMessageFormat = "All users: `{0}`\nUsers banned: `{1}`\nCan Send to `{2}` users";

        public const string ConfigApiKeyHasValueFormat = "Secret Key: *{0}*";
        public const string ConfigApiKeyHasNotValue = "You don't set any secret key";


        public static string UserInfo(long userId, bool isBan, DateTime createTime)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"*Id:* {userId}");
            builder.AppendLine($"*Is Ban:* {(isBan ? "YES" : "NO")}");
            builder.AppendLine($"*Create Time:* {createTime:G}");
            return builder.ToString();
        }

        public static string AccountInfo(int count, int maxCount)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"⏳Count: `{count}/{maxCount}`");
            return builder.ToString();
        }

        public static async Task<string> OSInfoText()
        {
            var osInfo = await SystemInformation.GetOSInfo();

            var builder = new StringBuilder();
            builder.AppendLine($"Platform: {osInfo.Platform}");
            builder.AppendLine($"Total Memory(OS): {osInfo.TotalMemorySize}");
            builder.AppendLine($"Used Memory(OS): {osInfo.UsedMemorySize}");
            return builder.ToString();
        }
    }
}
