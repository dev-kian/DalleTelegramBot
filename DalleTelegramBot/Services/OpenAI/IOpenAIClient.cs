using DalleTelegramBot.Services.OpenAI.Request;
using DalleTelegramBot.Services.OpenAI.Response;

namespace DalleTelegramBot.Services.OpenAI;

internal interface IOpenAIClient
{
    Task<ImageGenerationResponse> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken, string? apiKey = null!);
    Task<bool> ValidateApiKey(string apiKey, CancellationToken cancellationToken = default);
}
