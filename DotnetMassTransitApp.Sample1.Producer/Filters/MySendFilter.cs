using MassTransit;

namespace DotnetMassTransitApp.Sample1.Producer.Filters;

public class MySendFilter<T> :
    IFilter<SendContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        Console.WriteLine("MySendFilter init");

        await next.Send(context);

        Console.WriteLine("MySendFilter finalized");
    }
}
