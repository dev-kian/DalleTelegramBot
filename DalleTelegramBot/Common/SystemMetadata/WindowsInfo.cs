using DalleTelegramBot.Common.SystemMetadata.SharedData;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

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
    private readonly OSInfo _oSInfo;
    public UnixInfo()//todo: get from ctor and set in activator.instance
    {
        _oSInfo = new();
    }
    public async Task<OSInfo> GetOSInfo()
    {
        try
        {
            await InitializeAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Throw EX in {nameof(GetOSInfo)}.");
        }

        return _oSInfo;
    }

    private async Task InitializeAsync()
    {
        var platform = await ExecuteCommandAsync("uname");
        _oSInfo.Platform = platform;
        await InitMemoryOSAsync();
    }

    private async Task InitMemoryOSAsync()
    {
        var resultCommand = await ExecuteCommandAsync("free", "-k");
        Regex regex = new Regex(@"^Mem:\s+(\d+)\s+(\d+)", RegexOptions.Multiline);

        Match match = regex.Match(resultCommand);

        if (!match.Success)
            return;

        double totalMemory = Math.Round(Convert.ToDouble(match.Groups[1].Value), 2);
        double usedMemory = Math.Round(Convert.ToDouble(match.Groups[2].Value), 2);
        double freeMemory = Math.Round(totalMemory - usedMemory, 2);
        _oSInfo.TotalMemorySize = totalMemory;
        _oSInfo.UsedMemorySize = usedMemory;
        _oSInfo.FreeMemorySize = freeMemory;
    }

    private async Task<string> ExecuteCommandAsync(string command, string args = null!, CancellationToken cancellationToken = default)
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
