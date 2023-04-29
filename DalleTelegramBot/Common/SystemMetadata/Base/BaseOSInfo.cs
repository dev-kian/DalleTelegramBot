using DalleTelegramBot.Common.SystemMetadata.SharedData;
using System.Diagnostics;

namespace DalleTelegramBot.Common.SystemMetadata.Base;

internal abstract class BaseOSInfo : IOSInfo
{
    protected readonly OSDetails _oSDetails;
    public BaseOSInfo(OSDetails oSDetails)
    {
        _oSDetails = oSDetails;
        InitAppMemoryUsage();
    }

    public abstract Task<OSDetails> GetOSDetails();

    protected decimal ToGB(decimal input)
        => input / (1024 * 1024);

    private void InitAppMemoryUsage()
    {
        var memoryUsed = Math.Round(ToGB(Process.GetCurrentProcess().PrivateMemorySize64), 2);
        _oSDetails.AppMemoryUsageSize = memoryUsed;
    }
}
