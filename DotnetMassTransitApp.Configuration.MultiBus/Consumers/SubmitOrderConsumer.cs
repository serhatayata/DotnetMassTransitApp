using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.MultiBus.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        return Task.CompletedTask;
    }
}
