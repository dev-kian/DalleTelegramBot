namespace DalleTelegramBot.Configurations
{
    internal class BotConfig
    {
        private static bool status = true;
        public static void TurnOnBot() => status = true;
        public static void TurnOffBot() => status = false;
        public static bool BotIsOn() => status;


        public static int LimitCount = 5;
    }
}
