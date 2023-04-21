namespace DalleTelegramBot.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class ResponseQueryAttribute : Attribute
{
    public string Name { get; }
    public bool AdminRequired { get; }

    public ResponseQueryAttribute(string name, bool adminRequired = false)
    {
        Name = name;
        AdminRequired = adminRequired;
    }
}