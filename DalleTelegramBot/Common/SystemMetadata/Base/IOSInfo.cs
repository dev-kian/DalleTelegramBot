using DalleTelegramBot.Common.SystemMetadata.SharedData;

namespace DalleTelegramBot.Common.SystemMetadata.Base;

internal interface IOSInfo
{
    Task<OSDetails> GetOSDetails();
}
