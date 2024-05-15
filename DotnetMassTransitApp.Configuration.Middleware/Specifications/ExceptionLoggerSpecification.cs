using MassTransit.Configuration;
using MassTransit;
using DotnetMassTransitApp.Configuration.Middleware.Filters;

namespace DotnetMassTransitApp.Configuration.Middleware.Specifications;

public class ExceptionLoggerSpecification<T> :
    IPipeSpecification<T>
    where T : class, PipeContext
{
    public IEnumerable<ValidationResult> Validate()
    {
        return Enumerable.Empty<ValidationResult>();
    }

    public void Apply(IPipeBuilder<T> builder)
    {
        builder.AddFilter(new ExceptionLoggerFilter<T>());
    }
}
