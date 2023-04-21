namespace DalleTelegramBot.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class CommandAttribute : Attribute
{
    public string Name { get; }
    public bool AdminRequired { get; }

    public CommandAttribute(string name, bool adminRequired = false)
    {
        Name = name;
        AdminRequired = adminRequired;
    }
}
