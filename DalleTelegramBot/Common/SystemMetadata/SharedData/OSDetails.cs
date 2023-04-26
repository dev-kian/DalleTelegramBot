namespace DalleTelegramBot.Common.SystemMetadata.SharedData;

internal class OSDetails
{
    public string Platform { get; set; }
    public decimal TotalMemorySize { get; set; }
    public decimal UsedMemorySize { get; set; }
    public decimal FreeMemorySize { get; set; }
    public decimal AppMemoryUsageSize { get; set; }
}
