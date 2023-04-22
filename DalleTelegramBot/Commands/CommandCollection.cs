using DalleTelegramBot.Commands.Base;
using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.Enums;
using DalleTelegramBot.Common.IDependency;
using System.Collections.Concurrent;
using System.Reflection;

namespace DalleTelegramBot.Commands;

internal sealed class CommandCollection : ISingletonDependency
{
    private readonly ConcurrentDictionary<string, TypeDetails> _commands = new();
    public CommandCollection()
    {
        AddCommands();
    }
    private void AddCommands()
    {
        var commandType = typeof(ICommand);
        var commandTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && commandType.IsAssignableFrom(x));
        foreach (var type in commandTypes)
        {
            var attribute = type.GetCustomAttribute<CommandAttribute>();
            if (attribute is null) continue;
            _commands[attribute.Name] = new(type, attribute.Role);
        }
    }

    public bool TryGetCommand(string commandName, out TypeDetails commandType)
    {
        return _commands.TryGetValue(commandName, out commandType!);
    }

    public bool ContainsCommand(string commandName)
    {
        return _commands.ContainsKey(commandName);
    }

    public Role GetRoleCommand(string commandName) =>
        _commands[commandName].Role;
}

internal class TypeDetails
{
    public Type TypeModel { get; }
    public Role Role { get; }

    public TypeDetails(Type type, Role role)
    {
        TypeModel = type;
        Role = role;
    }
}