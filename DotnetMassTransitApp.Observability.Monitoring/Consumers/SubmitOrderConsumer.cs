using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Observability.Monitoring.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        return Task.CompletedTask;
    }
}
