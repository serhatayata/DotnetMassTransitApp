using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Consumer.Consumers;

public class RefundOrderConsumer : IConsumer<RefundOrder>
{
    public Task Consume(ConsumeContext<RefundOrder> context)
    {
        return Task.CompletedTask;
    }
}
