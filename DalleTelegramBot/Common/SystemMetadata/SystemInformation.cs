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

        return null!;
    }

    private static async Task<OSInfo> GetOSInfo<TOS>() where TOS : IOSInfo
    {
        return await Activator.CreateInstance<TOS>().GetOSInfo();
    }
}
