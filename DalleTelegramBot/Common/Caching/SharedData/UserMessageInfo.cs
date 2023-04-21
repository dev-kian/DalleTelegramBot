namespace DalleTelegramBot.Common.Caching.SharedData;

internal class UserMessageInfo
{
    public int MessageCount { get; set; }
    public DateTime LastMessageTime { get; set; }
    public string LastCommand { get; set; }
    public byte StateCommand { get; set; }
    public object[] Data { get; set; }
}
