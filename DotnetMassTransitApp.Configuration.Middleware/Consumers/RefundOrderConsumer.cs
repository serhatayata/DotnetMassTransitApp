using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Middleware.Consumers;

public class RefundOrderConsumer : IConsumer<RefundOrder>
{
    public Task Consume(ConsumeContext<RefundOrder> context)
    {
        throw new NullReferenceException();
    }
}
