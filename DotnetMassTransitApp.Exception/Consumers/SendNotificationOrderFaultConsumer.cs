using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Exception.Consumers;

public class SendNotificationOrderFaultConsumer : IConsumer<Fault<SendNotificationOrder>>
{
    public Task Consume(ConsumeContext<Fault<SendNotificationOrder>> context)
    {
        return Task.CompletedTask;
    }
}
