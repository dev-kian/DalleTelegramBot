namespace DalleTelegramBot.Common;
internal sealed class AppSettings
{
    public TelegramSettings TelegramSettings { get; set; }
    public OpenAISettings OpenAISettings { get; set; }
}

internal sealed class TelegramSettings
{
    public string Token { get; set; }
    public long AdminId { get; set; }
}

internal sealed class OpenAISettings
{
    public string ApiKey { get; set; }
    public string ResourceName { get; set; }
    public string ApiVersion { get; set; }
    public string ImageRoot { get; set; }
    public string EnginesRoot { get; set; }
    public string BaseRequest => $"/{ApiVersion}/";
    public string ImageEndpoint => $"https://{ResourceName}{BaseRequest}{ImageRoot}";
    public string EnginesEndpoint => $"https://{ResourceName}{BaseRequest}{EnginesRoot}";
}
