using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Middleware.Consumers;

public class StartDeliveryConsumer : IConsumer<StartDelivery>
{
    public Task Consume(ConsumeContext<StartDelivery> context)
    {
        return Task.CompletedTask;
    }
}
