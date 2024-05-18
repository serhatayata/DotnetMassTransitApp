using MassTransit;

namespace DotnetMassTransitApp.Sample1.Producer.Filters;

public class MyPublishFilter<T> :
    IFilter<PublishContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        Console.WriteLine("MyPublishFilter init");

        await next.Send(context);

        Console.WriteLine("MyPublishFilter finalized");
    }
}
