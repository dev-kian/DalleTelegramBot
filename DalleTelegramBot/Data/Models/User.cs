using DalleTelegramBot.Common.Enums;

namespace DalleTelegramBot.Data.Models;

internal class User
{
    public required long Id { get; set; }
    public bool IsBan { get; set; }
    public DateTime CreateTime { get; set; } = DateTime.UtcNow;
    public string ApiKey { get; set; }
    public ImageSize ImageSize { get; set; }
    public int ImageCount { get; set; } = 1;
}