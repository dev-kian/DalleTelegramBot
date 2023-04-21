using DalleTelegramBot.Common;
using DalleTelegramBot.Services.OpenAI.Request;
using DalleTelegramBot.Services.OpenAI.Response;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using DalleTelegramBot.Common.Extensions;

namespace DalleTelegramBot.Services.OpenAI
{
    internal class OpenAIClient : IOpenAIClient, IDisposable
    {
        private readonly OpenAISettings _settings;
        private readonly HttpClient _client;
        private readonly JsonSerializerOptions _jsonOptions;
        public OpenAIClient(OpenAISettings settings, HttpClient client, JsonSerializerOptions jsonOptions = null!)
        {
            _settings = settings;
            _client = client;

            jsonOptions ??= new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            _jsonOptions = jsonOptions;
        }

        public async Task<ImageGenerationResponse> GenerateImageAsync(ImageGenerationRequest request, CancellationToken cancellationToken, string? apiKey = null)
        {
            apiKey ??= _settings.ApiKey;

            var json = JsonSerializer.Serialize(request, _jsonOptions);

            using var requestMessage = new HttpRequestMessage
            {
                RequestUri = new(_settings.ImageEndpoint),
                Method=HttpMethod.Post,
                Content = new StringContent(json, Encoding.UTF8, "application/json"),
                Headers =
                {
                    { "User-Agent", "OpenAI-DotNet" },
                    { "Authorization", $"Bearer {apiKey}" },
                },
            };
            var response = await _client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            var imageGenerationResponse = new ImageGenerationResponse() { IsSuccess = response.IsSuccessStatusCode };
            if (response.IsSuccessStatusCode)
            {
                var imagesResponse = JsonSerializer.Deserialize<ImagesResponse>(responseString, _jsonOptions);
                if (imagesResponse?.Data is null || imagesResponse.Data.Count is 0)
                    throw new HttpRequestException($"{nameof(GenerateImageAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {responseString}");

                imagesResponse.SetResponseData(response.Headers);

                imageGenerationResponse.Images = imagesResponse;
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseString, _jsonOptions);
                if(errorResponse is null)
                    throw new HttpRequestException($"{nameof(GenerateImageAsync)} returned no results!  HTTP status code: {response.StatusCode}. Response body: {responseString}");

                errorResponse.SetResponseData(response.Headers);

                imageGenerationResponse.Error = errorResponse;
            }

            return imageGenerationResponse;
        }

        public async Task<bool> ValidateApiKey(string apiKey, CancellationToken cancellationToken = default)
        {
            using var requestMessage = new HttpRequestMessage
            {
                RequestUri = new(_settings.EnginesEndpoint),
                Method=HttpMethod.Get,
                Headers =
                {
                    { "User-Agent", "OpenAI-DotNet" },
                    { "Authorization", $"Bearer {apiKey}" },
                },
            };

            var response = await _client.SendAsync(requestMessage, cancellationToken).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
