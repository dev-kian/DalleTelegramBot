using System.Text.Json.Serialization;

namespace DalleTelegramBot.Services.OpenAI.Response;

internal class ErrorResponse : BaseResponse
{
    [JsonInclude]
    [JsonPropertyName("error")]
    public ErrorResult Error { get; private set; }
}
