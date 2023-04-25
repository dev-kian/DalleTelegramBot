using DalleTelegramBot.Common.SystemMetadata.SharedData;

namespace DalleTelegramBot.Common.SystemMetadata.Base;

internal abstract class BaseOSInfo : IOSInfo
{
    protected readonly OSDetails _oSDetails;
    public BaseOSInfo(OSDetails oSDetails)
    {
        _oSDetails = oSDetails;
    }

    public abstract Task<OSDetails> GetOSDetails();

    protected decimal ToGB(decimal input)
        => input / (1024 * 1024);
}
