using MassTransit.Configuration;
using MassTransit;
using DotnetMassTransitApp.Configuration.Middleware.Filters;

namespace DotnetMassTransitApp.Configuration.Middleware.Specifications;

public class MessageFilterPipeSpecification<T> :
    IPipeSpecification<ConsumeContext<T>>
    where T : class
{
    public void Apply(IPipeBuilder<ConsumeContext<T>> builder)
    {
        var filter = new MessageFilter<T>();

        builder.AddFilter(filter);
    }

    public IEnumerable<ValidationResult> Validate()
    {
        yield break;
    }
}