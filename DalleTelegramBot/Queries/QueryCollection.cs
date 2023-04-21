using DalleTelegramBot.Common.Attributes;
using DalleTelegramBot.Common.IDependency;
using DalleTelegramBot.Queries.Base;
using System.Collections.Concurrent;
using System.Reflection;

namespace DalleTelegramBot.Queries;

internal sealed class QueryCollection : ISingletonDependency
{
    private readonly ConcurrentDictionary<string, Type> _queries = new();
    public QueryCollection()
    {
        AddQueries();
    }
    private void AddQueries()
    {
        var queryType = typeof(IQuery);
        var queryTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(x => x.IsClass && !x.IsAbstract && queryType.IsAssignableFrom(x));
        foreach (var type in queryTypes)
        {
            var attribute = type.GetCustomAttribute<QueryAttribute>();
            if (attribute is null) continue;
            _queries[attribute.Name] = type;
        }
    }

    public bool TryGetQuery(string queryName, out Type? queryType)
    {
        return _queries.TryGetValue(queryName, out queryType);
    }
}
