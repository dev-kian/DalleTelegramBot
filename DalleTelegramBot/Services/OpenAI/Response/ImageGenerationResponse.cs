namespace DalleTelegramBot.Services.OpenAI.Response;

internal class ImageGenerationResponse
{
    public required bool IsSuccess { get; set; }
    public ImagesResponse? Images { get; set; }
    public ErrorResponse? Error { get; set; }
}
