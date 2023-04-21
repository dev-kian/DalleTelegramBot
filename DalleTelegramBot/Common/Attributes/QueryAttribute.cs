namespace DalleTelegramBot.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class QueryAttribute : Attribute
{
    public string Name { get; }

    public QueryAttribute(string name)
    {
        Name = name;
    }
}
