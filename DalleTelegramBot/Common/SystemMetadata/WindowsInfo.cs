using DalleTelegramBot.Common.SystemMetadata.SharedData;
using System.Diagnostics;

namespace DalleTelegramBot.Common.SystemMetadata;

internal class WindowsInfo : IOSInfo
{
    public async Task<OSInfo> GetOSInfo()
    {
        return new()
        {
            Platform = "Windows"
        };
    }

    
}

internal class UnixInfo : IOSInfo
{
    public async Task<OSInfo> GetOSInfo()
    {
        var platform = await ExecuteCommandTerminal("uname");
        return new()
        {
            Platform = platform,
        };
    }

    private async Task<string> ExecuteCommandTerminal(string command, string args = null!, CancellationToken cancellationToken = default)
    {
        var processInfo = new ProcessStartInfo(command, args)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true
        };

        using var process = Process.Start(processInfo);

        var output = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync(cancellationToken);

        return output;
    }
}
