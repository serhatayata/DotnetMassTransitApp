using MassTransit;
using Shared.Queue.Events;

namespace DotnetMassTransitApp.Patterns.Saga.Consumer.Consumers;

public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
{
    public Task Consume(ConsumeContext<OrderSubmitted> context)
    {
        return Task.CompletedTask;
    }
}
