using DotnetMassTransitApp.Configuration.Middleware.Specifications;
using MassTransit;

namespace DotnetMassTransitApp.Configuration.Middleware.Extensions;

public static class ExampleMiddlewareConfiguratorExtensions
{
    public static void UseExceptionLogger<T>(this IPipeConfigurator<T> configurator)
        where T : class, PipeContext
    {
        configurator.AddPipeSpecification(new ExceptionLoggerSpecification<T>());
    }
}
