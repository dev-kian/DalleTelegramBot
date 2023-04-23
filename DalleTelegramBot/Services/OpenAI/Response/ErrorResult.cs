using System.Text.Json.Serialization;

namespace DalleTelegramBot.Services.OpenAI.Response;

internal class ErrorResult
{
    [JsonInclude]
    [JsonPropertyName("code")]
    public string Code { get; private set; }
    [JsonInclude]
    [JsonPropertyName("message")]
    public string Message { get; private set; }
    [JsonInclude]
    [JsonPropertyName("param")]
    public string Param { get; private set; }
    [JsonInclude]
    [JsonPropertyName("type")]
    public string Type { get; private set; }
}
