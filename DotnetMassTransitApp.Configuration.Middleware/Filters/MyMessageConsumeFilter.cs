using MassTransit;
using Shared.Queue.Contracts;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace DotnetMassTransitApp.Configuration.Middleware.Filters;

public class MyMessageConsumeFilter :
    IFilter<ConsumeContext<RefundOrder>>,
    IFilter<ConsumeContext<StartDelivery>>
{
    public async Task Send(ConsumeContext<StartDelivery> context, IPipe<ConsumeContext<StartDelivery>> next)
    {
        Console.WriteLine("MyMessageConsumeFilter start delivery order init");

        await next.Send(context);

        Console.WriteLine("MyMessageConsumeFilter start delivery order ended");
    }

    public async Task Send(ConsumeContext<RefundOrder> context, IPipe<ConsumeContext<RefundOrder>> next)
    {
        Console.WriteLine("MyMessageConsumeFilter refund order init");

        await next.Send(context);

        Console.WriteLine("MyMessageConsumeFilter refund order ended");
    }

    public void Probe(ProbeContext context)
    {

    }
}
