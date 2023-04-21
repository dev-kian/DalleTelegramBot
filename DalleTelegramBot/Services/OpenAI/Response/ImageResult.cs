using System.Text.Json.Serialization;

namespace DalleTelegramBot.Services.OpenAI.Response;

internal class ImageResult
{
    [JsonInclude]
    [JsonPropertyName("url")]
    public string Url { get; private set; }
}
