using DalleTelegramBot.Services.OpenAI.Response;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;

namespace DalleTelegramBot.Common.Extensions;

internal static class OpenAIExtensions
{
    private const string ORGANAIZATION = "Openai-Organization";
    private const string REQUESTID = "X-Request-ID";
    private const string PROCESSINGTIME = "Openai-Processing-Ms";

    public static void SetResponseData(this BaseResponse response, HttpResponseHeaders headers)
    {
        if (headers.TryGetValues(ORGANAIZATION, out var organizationValues))
        {
            response.Organization = organizationValues?.FirstOrDefault();
        }
        if (headers.TryGetValues(PROCESSINGTIME, out var processingTimeValues) && double.TryParse(processingTimeValues?.FirstOrDefault(), out var processingTime))
        {
            response.ProcessingTime = TimeSpan.FromMilliseconds(processingTime);
        }
        if (headers.TryGetValues(REQUESTID, out var requestIdValues))
        {
            response.RequestId = requestIdValues?.FirstOrDefault();
        }
    }

    public static bool ValidateApiKey(this string input)
    {
        return Regex.IsMatch(input, @"^sk-[a-zA-Z0-9]{32,}$");
    }

    public static string MaskApiKey(this string input)
    {
        return Regex.Replace(input, @"^(sk-).{3}.*(.{4})$", "$1...$2");
    }
}