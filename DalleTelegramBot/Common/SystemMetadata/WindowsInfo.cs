using DalleTelegramBot.Common.SystemMetadata.Base;
using DalleTelegramBot.Common.SystemMetadata.SharedData;
using Microsoft.Win32;
using Serilog;
using System.Management;
using System.Text.RegularExpressions;

namespace DalleTelegramBot.Common.SystemMetadata;

internal class WindowsInfo : BaseOSInfo
{
    public WindowsInfo(OSDetails oSDetails) : base(oSDetails) { }

    public override async Task<OSDetails> GetOSDetails()
    {
        try
        {
            InitPlatform();
            InitMemoryOS();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error occurred while trying to get OS details.");
        }

        return _oSDetails;
    }
    private void InitPlatform()
    {
        var versionName = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ProductName", "Unknown")?.ToString();
        var architecture = Environment.Is64BitOperatingSystem ? "x64" : "x86";
        _oSDetails.Platform = $"{versionName} {architecture}";
    }

    private void InitMemoryOS()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        var query = new ObjectQuery("SELECT TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
        var searcher = new ManagementObjectSearcher(query);
        var result = searcher.Get().Cast<ManagementObject>().FirstOrDefault();

        var totalMemory = Convert.ToDecimal(result["TotalVisibleMemorySize"]);
        var freeMemory = Convert.ToDecimal(result["FreePhysicalMemory"]);
        var usedMemory = totalMemory - freeMemory;
        _oSDetails.TotalMemorySize = Math.Round(ToGB(totalMemory), 2);
        _oSDetails.UsedMemorySize = Math.Round(ToGB(usedMemory), 2);
        _oSDetails.FreeMemorySize = Math.Round(ToGB(freeMemory), 2);
#pragma warning restore CA1416 // Validate platform compatibility
    }
}