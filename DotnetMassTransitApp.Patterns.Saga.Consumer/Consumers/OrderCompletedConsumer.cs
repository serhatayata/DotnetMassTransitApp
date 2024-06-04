using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Patterns.Saga.Consumer.Consumers;

public class OrderCompletedConsumer : IConsumer<OrderCompleted>
{
    public Task Consume(ConsumeContext<OrderCompleted> context)
    {
        return Task.CompletedTask;
    }
}
