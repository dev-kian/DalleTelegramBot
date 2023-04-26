using DalleTelegramBot.Common.SystemMetadata.Base;
using DalleTelegramBot.Common.SystemMetadata.SharedData;
using Serilog;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DalleTelegramBot.Common.SystemMetadata;

internal class UnixInfo : BaseOSInfo
{
    public UnixInfo(OSDetails oSDetails) : base(oSDetails) { }

    public override async Task<OSDetails> GetOSDetails()
    {
        try
        {
            await InitializeAsync();
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Error occurred while trying to get OS details.");
        }

        return _oSDetails;
    }

    private async Task InitializeAsync()
    {
        var platform = await ExecuteCommandAsync("uname");
        _oSDetails.Platform = platform;
        await InitMemoryOSAsync();
    }

    private async Task InitMemoryOSAsync()
    {
        var resultCommand = await ExecuteCommandAsync("free", "-k");
        Regex regex = new Regex(@"^Mem:\s+(\d+)\s+(\d+)", RegexOptions.Multiline);

        Match match = regex.Match(resultCommand);

        if (!match.Success)
            return;

        var totalMemory = Convert.ToDecimal(match.Groups[1].Value);
        var usedMemory = Convert.ToDecimal(match.Groups[2].Value);
        var freeMemory = totalMemory - usedMemory;
        _oSDetails.TotalMemorySize = Math.Round(ToGB(totalMemory), 2);
        _oSDetails.UsedMemorySize = Math.Round(ToGB(usedMemory), 2);
        _oSDetails.FreeMemorySize = Math.Round(ToGB(freeMemory),2);
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
