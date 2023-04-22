using DalleTelegramBot.Common.Enums;

namespace DalleTelegramBot.Common.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
internal sealed class CommandAttribute : Attribute
{
    public string Name { get; }
    public Role Role { get; }

    public CommandAttribute(string name, Role role = Role.Optional)
    {
        Name = name;
        Role = role;
    }
}
