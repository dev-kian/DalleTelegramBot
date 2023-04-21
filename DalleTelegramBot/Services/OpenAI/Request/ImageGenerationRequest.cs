using DalleTelegramBot.Common.Enums;
using System.Text.Json.Serialization;

namespace DalleTelegramBot.Services.OpenAI.Request;

internal class ImageGenerationRequest
{
    public ImageGenerationRequest(string prompt, int numberOfResults, ImageSize imageSize)
    {
        if (prompt.Length > 1000)
        {
            throw new ArgumentOutOfRangeException(nameof(prompt), "The maximum character length for the prompt is 1000 characters.");
        }

        if (numberOfResults is > 10 or < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfResults), "The number of results must be between 1 and 10");
        }

        Prompt = prompt;
        Number = numberOfResults;
        Size = imageSize switch
        {
            ImageSize.Small => "256x256",
            ImageSize.Medium => "512x512",
            ImageSize.Large => "1024x1024",
            _ => throw new ArgumentOutOfRangeException(nameof(imageSize), imageSize, null)
        };
    }

    [JsonPropertyName("n")]
    public int Number { get; }

    [JsonPropertyName("response_format")]
    public string Format { get; } = "url";

    [JsonPropertyName("size")]
    public string Size { get; }

    [JsonPropertyName("prompt")]
    public string Prompt { get; }
}
