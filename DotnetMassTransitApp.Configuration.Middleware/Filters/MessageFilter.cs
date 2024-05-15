using MassTransit;

namespace DotnetMassTransitApp.Configuration.Middleware.Filters;

public class MessageFilter<T> :
    IFilter<ConsumeContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
        var scope = context.CreateFilterScope("messageFilter");
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        // do something

        await next.Send(context);
    }
}