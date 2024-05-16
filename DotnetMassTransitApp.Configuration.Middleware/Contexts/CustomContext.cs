using MassTransit;

namespace DotnetMassTransitApp.Configuration.Middleware.Contexts;

public interface CustomContext : PipeContext
{
    string SomeThing { get; }
}
