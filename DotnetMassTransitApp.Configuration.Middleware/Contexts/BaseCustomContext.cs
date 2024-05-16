using MassTransit.Middleware;

namespace DotnetMassTransitApp.Configuration.Middleware.Contexts;

public class BaseCustomContext :
    BasePipeContext,
    CustomContext
{
    public string SomeThing { get; set; }
}
