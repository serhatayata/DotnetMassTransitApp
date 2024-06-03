using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Patterns.Saga.Consumer.Consumers;

public class UpdateAccountHistoryConsumer : IConsumer<UpdateAccountHistory>
{
    public Task Consume(ConsumeContext<UpdateAccountHistory> context)
    {
        return Task.CompletedTask;
    }
}
