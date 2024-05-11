using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public SubmitOrderConsumer()
    {
    }

    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        return Task.CompletedTask;
    }
}