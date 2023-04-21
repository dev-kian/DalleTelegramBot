using DalleTelegramBot.Filters;

namespace DalleTelegramBot.Common.Attributes;

internal sealed class CheckBanUserAttribute : FilterAttribute
{
    public CheckBanUserAttribute() : base(typeof(CheckBanUserFilter))
    {
    }
}
