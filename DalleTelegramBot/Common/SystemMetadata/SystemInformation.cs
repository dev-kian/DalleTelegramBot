using DalleTelegramBot.Common.SystemMetadata.SharedData;
using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.RuntimeInformation;


namespace DalleTelegramBot.Common.SystemMetadata;

internal class SystemInformation
{
    public static async Task<OSInfo> GetOSInfo()
    {
        if(IsOSPlatform(OSPlatform.Windows))
        {
            return await GetOSInfo<WindowsInfo>();
        }
        else if(IsOSPlatform(OSPlatform.Linux) || IsOSPlatform(OSPlatform.FreeBSD)) 
        {
            return await GetOSInfo<UnixInfo>();
        }

        return CreateEmptyOSInfo();
    }

    private static async Task<OSInfo> GetOSInfo<TOS>() where TOS : IOSInfo
    {
        var instance = Activator.CreateInstance(typeof(TOS), args: new[] { CreateEmptyOSInfo() });
        return await (instance as IOSInfo)!.GetOSInfo();
    }

    private static OSInfo CreateEmptyOSInfo()
    {
        var emptyOSInfo = new OSInfo()
        {
            Platform = "Unknown",
            TotalMemorySize = -1,
            UsedMemorySize = -1,
            FreeMemorySize = -1,
        };

        return emptyOSInfo;
    }
}
