using DalleTelegramBot.Filters.Base;

namespace DalleTelegramBot.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
internal class FilterAttribute : Attribute
{
    public Type FilterType { get; }
    public FilterAttribute(Type filterType)
    {
        if (!typeof(IFilter).IsAssignableFrom(filterType))
        {
            throw new ArgumentException($"{filterType.Name} must implement IFilter.");
        }

        FilterType = filterType;
    }
}
