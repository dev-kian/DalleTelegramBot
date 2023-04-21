using System.Text.Json.Serialization;

namespace DalleTelegramBot.Services.OpenAI.Response;

internal abstract class BaseResponse
{
    [JsonIgnore]
    public TimeSpan ProcessingTime { get; set; }
    [JsonIgnore]
    public string Organization { get; set; }
    [JsonIgnore]
    public string RequestId { get; set; }
}
