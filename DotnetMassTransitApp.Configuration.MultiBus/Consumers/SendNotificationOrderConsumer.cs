using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.MultiBus.Consumers;

public class SendNotificationOrderConsumer : IConsumer<SendNotificationOrder>
{
    public Task Consume(ConsumeContext<SendNotificationOrder> context)
    {
        return Task.CompletedTask;
    }
}
