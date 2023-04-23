using Telegram.Bot.Types.ReplyMarkups;

namespace DalleTelegramBot.Common.Utilities
{
    internal class InlineUtility
    {
        public static InlineKeyboardMarkup AdminSettingsInlineKeyboard = new(new[]
        {
            KeyboardButton("👥 All Users", "all-users 0"),
            KeyboardButton("👥 All Users(Banned)", "all-users 0 x"),
            KeyboardButton("🔎 Search User", "search-user"),
            KeyboardButton("🗣 Communicate", "communicate"),
            KeyboardButton("🔧 Bot Config", "bot-config"),
        });

        public static InlineKeyboardMarkup AccountSettingsInlineKeyboard = new(new[]
        {
                KeyboardButton("🗝 Config Api Key", "config-api-key"),
                KeyboardButton("Config Image Generation", "config-img-generation"),
        });

        public static InlineKeyboardMarkup AccountSettingsImageCountInlineKeyboard(int imgCount)
        {
            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();
            for (int i = 0; i < 9; i += 3)
            {
                var inlineRow = new List<InlineKeyboardButton>();
                for (int j = 1; j <= 3; j++)
                {
                    int buttonNumber = i + j;
                    string buttonText = $"{buttonNumber}️⃣ {(imgCount == buttonNumber ? " ✔️" : "")}";
                    string callbackData = $"config-img-count {buttonNumber}";
                    inlineRow.Add(InlineKeyboardButton.WithCallbackData(buttonText, callbackData));
                }
                keyboardRows.Add(inlineRow);
            }

            keyboardRows.Add(BackKeyboardButton("account"));

            return new(keyboardRows);
        }

        public static InlineKeyboardMarkup AccountSettingsImageSizeInlineKeyboard(int imgSize)
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                KeyboardButton($"Small(256x256){(imgSize == 0 ? " ✅" : string.Empty)}", "config-img-size 0"),
                KeyboardButton($"Medium(512x512){(imgSize == 1 ? " ✅" : string.Empty)}", "config-img-size 1"),
                KeyboardButton($"Large(1024x1024){(imgSize == 2 ? " ✅" : string.Empty)}", "config-img-size 2"),
                BackKeyboardButton("account"),
            });

            return inlineKeyboard;
        }

        public static InlineKeyboardMarkup AccountSettingsApiKeyInlineKeyboard(bool hasApiKey)
        {
            List<IEnumerable<InlineKeyboardButton>> inlineKeyboardButtons = new();
            if (hasApiKey)
                inlineKeyboardButtons.Add(KeyboardButton("Remove Api Key ❌", "config-api-key remove"));
            else
                inlineKeyboardButtons.Add(KeyboardButton("Set Api Key ✅", "config-api-key set"));
            inlineKeyboardButtons.Add(BackKeyboardButton("account"));

            return new(inlineKeyboardButtons);
        }


        public static InlineKeyboardMarkup AdminSettingsBanUserInlineKeyboard(long userId, bool isBan, bool hasBackButton = true)
        {
            List<IEnumerable<InlineKeyboardButton>> inlineKeyboardButtons = new()
            {
                KeyboardButton(isBan ? "Unban User" : "Ban user", $"ban-user {userId}{(hasBackButton ? " x" : "")}"),
            };
            if (hasBackButton)
                inlineKeyboardButtons.Add(BackKeyboardButton("settings"));

            return new(inlineKeyboardButtons);
        }

        public static IEnumerable<InlineKeyboardButton> KeyboardButton(string text, string callbackData) =>
            new[] { InlineKeyboardButton.WithCallbackData(text, callbackData) };

        public static IEnumerable<InlineKeyboardButton> BackKeyboardButton(string callback = "")
            => new[] { InlineKeyboardButton.WithCallbackData("🔙Back", $"back-{callback}") };



        public static ReplyKeyboardMarkup StartCommandReplyKeyboard = new(new[]
        {
            new KeyboardButton[] { "Create Image🖼" },
            //new KeyboardButton[] { "Edit Image🖼" },
            new KeyboardButton[] { "Account⚙️" },
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
    }
}
