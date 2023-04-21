using DalleTelegramBot.Common.SystemMetadata.SharedData;

namespace DalleTelegramBot.Common.SystemMetadata;

internal interface IOSInfo
{
    Task<OSInfo> GetOSInfo();
}
