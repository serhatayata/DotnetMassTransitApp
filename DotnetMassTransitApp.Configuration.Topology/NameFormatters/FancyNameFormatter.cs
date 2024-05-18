using MassTransit;
using Shared.Queue.Events;

namespace DotnetMassTransitApp.Configuration.Topology.NameFormatters;

public class FancyNameFormatter<T> :
    IMessageEntityNameFormatter<T> 
    where T : class
{
    public string FormatEntityName()
    {
        return typeof(T).Name.ToString();
    }
}

public class FancyNameFormatter :
    IEntityNameFormatter
{
    private readonly IEntityNameFormatter _original;

    public FancyNameFormatter(IEntityNameFormatter original)
    {
        _original = original;
    }

    public string FormatEntityName<T>()
    {
        if (typeof(T) == typeof(OrderSubmitted))
            return "we-got-one";

        return _original.FormatEntityName<T>();
    }
}