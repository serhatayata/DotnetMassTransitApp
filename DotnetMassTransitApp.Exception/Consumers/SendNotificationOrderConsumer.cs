using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Exception.Consumers;

public class SendNotificationOrderConsumer : IConsumer<SendNotificationOrder>
{
    public async Task Consume(ConsumeContext<SendNotificationOrder> context)
    {
        throw new NotImplementedException("Error exceptions");
    }
}
