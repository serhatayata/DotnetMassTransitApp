using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Middleware.Filters;

public class MyCommandFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class, ICommandFilter
{
    public void Probe(ProbeContext context)
    {
        
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        Console.WriteLine("MyCommandFilter init");

        await next.Send(context);

        Console.WriteLine("MyCommandFilter ended");
    }
}
