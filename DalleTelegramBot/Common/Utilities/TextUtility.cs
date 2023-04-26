using DalleTelegramBot.Common.SystemMetadata;
using System.Text;

namespace DalleTelegramBot.Common.Utilities
{
    internal class TextUtility
    {
        public static string StartInfo(long userId, string name, bool newUser)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"👋🏻 {(newUser ? "Hello" : "Hello again")}! <b><a href=\"tg://user?id={userId}\">{name}</a></b>");
            builder.AppendLine("Welcome to <b>DALLE-2</b> bot! We're excited to have you here.\n We'd like to let you know that our project is open source and available on GitHub for you to view and contribute. By visiting our GitHub page at <b><a href=\"https://github.com/dev-kian/DalleTelegramBot\">link</a></b>, you can get a better understanding of how our bot works, suggest new features or report issues. Thank you for using our bot and we hope you find it helpful!");
            builder.AppendLine();
            builder.AppendLine("<i>• Write a sentence for me to create a picture of it for you.</i>");
            builder.AppendLine();
            builder.AppendLine("<i>• You can configure your API key in the bot and take pictures without any restrictions.</i>");
            builder.AppendLine();
            builder.AppendLine("<i>• Please don't use awkward words to create photos</i>");

            return builder.ToString();
        }

        public const string GetUserCommandNotValidUserId = "Your input is invalid! Please enter a valid user Id.";
        public const string GetUserCommandNotFoundUserIdFormat = "User Id {0} was not found.";

        public const string SearchUserCommandAsk = "Please enter the user Id you would like to search for.";

        public const string CommunicateQueryMessageFormat = "Number of users: `{0}`\nNumber of banned users: `{1}`\nNumber of users who can receive messages: `{2}`";

        public const string ConfigApiKeyHasValueFormat = "Secret Key: *{0}*";
        public const string ConfigApiKeyHasNotValue = "You haven't set a secret key yet.";

        public const string CommunicateCommandMessageFormat = "Your message has been successfully forwarded to {0} ({1}).";
        public const string CommunicateCommandStartForwardingMessageFormat = "Start forwarding your message to {0} users.";
        public const string CommunicateCommandSendMessage = "Please enter the message you would like to send.";
        public const string CommunicateCommandEndForwardMessageFormat = "Your message has been successfully forwarded to {0} users.\nFailed to send to: {1} users.";

        public const string BotOffMessage = "Sorry, the bot is currently offline or undergoing maintenance. We apologize for the inconvenience and will be back online as soon as possible. Thank you for your patience!";
        public const string UnknownCommandMessage = "Unfortunately, we are unable to understand your command.";
        public const string NotExistsCommandMessage = "Sorry, your command does not exist.\nUse the buttons below:";
        public const string NotExistsCommandAdminMessage = "Dear admin, your command does not exist.\nSend /start to access panel";
        public const string ExecuteByUserOnlyMessage = "This command can only be executed by users.";
        public const string ExceptionHappenedMessage = "⚠️Unfortunately, a problem has occurred in the implementation of this operation.\nContact support if needed.";

        public const string ConfigApiKeyBadFormatMessage = "Invalid API key format.";
        public const string ConfigApiKeyBadRequestMessage = "Unauthorized API key.";

        public const string ImgGenerationProcessingMessage = "⏳Processing\n_please wait..._";
        public const string ImgGenerationCompletedMessageFormat = "⌛️Completed✅\n({0})";
        public const string ImgGenerationExceededMessage = $"You can't generate image in 24 hours ago.{ImgGenerationWithoutLimitMessage}";
        public const string ImgGenerationLimitGenMessage = "You can only make {0} more photos." + ImgGenerationWithoutLimitMessage;
        public const string ImgGenerationBadApiKeyMessage = "Sorry, there was an error with your API key. OpenAI was unable to authorize your key.";
        public const string ImgGenerationSendPromptMessage = "Please send a prompt to generate your image";
        public const string ImgGenerationCancelMessage = "♻️The image creation operation was successfully canceled.";
        public const string ImgGenerationNotFinishingTaskMessage = "🚫Please wait until the previous task is completed.";
        public const string ImgGenerationCannotCancelMessage = "🚫You cannot cancel the task at this time, please wait until the task is finished.";
        private const string ImgGenerationWithoutLimitMessage = "\n\n_♨️If you want to use the bot without restrictions, you can set your personal api key in the Account > Config Api Key settings._";

        public const string BotLogNotFoundDirectory = "logs directory could not be found or it may not exist";

        public const string ConfigApiKeyGiveSecKey = "Please send your api key";


        public static string UserInfo(long userId, bool isBan, DateTime createTime)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"🔑<i>ID</i>: <b><a href=\"tg://user?id={userId}\">{userId}</a></b>");
            builder.AppendLine($"🐣<i>Registration date</i>: {createTime:G}");
            builder.AppendLine($"⚰️<i>Is Ban</i>: {(isBan ? "YES" : "NO")}");
            return builder.ToString();
        }

        public static string AccountInfo(string name, long userId, DateTime createTime, int count, int maxCount)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"☃️ <b>Profile</b>");
            builder.AppendLine();
            builder.AppendLine($"🗣<i>Name</i>: <b><a href=\"tg://user?id={userId}\">{name}</a></b>");
            builder.AppendLine($"🔑<i>ID</i>: {userId}");
            builder.AppendLine($"🐣<i>Registration date</i>: {createTime:yyyy-MM-dd}");
            builder.AppendLine();
            builder.AppendLine($"⏳<i>Count</i>: <code>{count}/{maxCount}</code>");
            return builder.ToString();
        }

        public static async Task<string> OSInfoText()
        {
            var osInfo = await SystemInformation.GetOSInfo();

            var builder = new StringBuilder();
            builder.AppendLine($"_Platform_: *{osInfo.Platform}*");
            builder.AppendLine($"_Total Memory(OS)_: {osInfo.TotalMemorySize}");
            builder.AppendLine($"_Used Memory(OS)_: {osInfo.UsedMemorySize}");
            builder.AppendLine($"_Free Memory(OS)_: {osInfo.FreeMemorySize}");
            builder.AppendLine($"_App Memory Usage_: {osInfo.AppMemoryUsageSize}");
            return builder.ToString();
        }
    }
}
