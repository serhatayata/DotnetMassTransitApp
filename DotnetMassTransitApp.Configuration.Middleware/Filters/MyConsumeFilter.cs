using MassTransit;

namespace DotnetMassTransitApp.Configuration.Middleware.Filters;

public class MyConsumeFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
        
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        Console.WriteLine("MyConsumerFilter entrance");

        await next.Send(context);

        Console.WriteLine("MyConsumerFilter exit");
    }
}
