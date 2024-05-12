using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Consumer.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        return Task.CompletedTask;
    }
}
