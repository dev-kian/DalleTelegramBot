using DalleTelegramBot.Common.SystemMetadata;
using System.Text;

namespace DalleTelegramBot.Common.Utilities
{
    internal class TextUtilitiy
    {
        public const string StartCommandExistsUser = "Before register";
        public const string StartCommandNotExistsUser = "New user\nRegister...";

        public const string GetUserCommandNotValidUserId = "Your input is valid!\nEnter digits user id";
        public const string GetUserCommandNotFoundUserIdFormat = "Id {0} was not found!";
        
        
        public const string SearchUserCommandAsk = "Please send user id to search";

        public const string CommunicateQueryMessageFormat = "All users: `{0}`\nUsers banned: `{1}`\nCan Send to `{2}` users";

        public const string ConfigApiKeyHasValueFormat = "Secret Key: *{0}*";
        public const string ConfigApiKeyHasNotValue = "You don't set any secret key";


        public const string CommunicateCommandMessageFormat = "Your message has been successfully forwarded to {0} ({1})";
        public const string CommunicateCommandStartForwardingMessageFormat = "Start forwarding your message to *{0}* users";
        public const string CommunicateCommandSendMessage = "Send your message";
        public const string CommunicateCommandEndForwardMessageForamt  = "Your message has been successfully forwarded to *{0}* users\nFailed can't send: *{1}*";

        public static string UserInfo(long userId, bool isBan, DateTime createTime)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"🔑_ID_: {userId}");
            builder.AppendLine($"⚰️_Is Ban_: {(isBan ? "YES" : "NO")}");
            builder.AppendLine($"🐣_Registration date_: {createTime:G}");
            return builder.ToString();
        }

        public static string AccountInfo(string name, long userId, DateTime createTime, int count, int maxCount)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"☃️ *Profile*");
            builder.AppendLine();
            builder.AppendLine($"🗣_Name_: {name}");
            builder.AppendLine($"🔑_ID_: {userId}");
            builder.AppendLine($"🐣_Registration date_: {createTime:yyyy-MM-dd}");
            builder.AppendLine();
            builder.AppendLine($"⏳_Count_: `{count}/{maxCount}`");
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
