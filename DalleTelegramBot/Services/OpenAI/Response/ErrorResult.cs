using System.Text.Json.Serialization;

namespace DalleTelegramBot.Services.OpenAI.Response;

internal class ErrorResult
{
    [JsonInclude]
    public string Code { get; private set; }
    [JsonInclude]
    public string Message { get; private set; }
    [JsonInclude]
    public string Param { get; private set; }
    [JsonInclude]
    public string Type { get; private set; }
}
