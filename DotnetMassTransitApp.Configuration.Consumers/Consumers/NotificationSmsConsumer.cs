using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Consumers.Consumers;

public class NotificationSmsConsumer : IConsumer<NotificationSms>
{
    public Task Consume(ConsumeContext<NotificationSms> context)
    {
        return Task.CompletedTask;
    }
}
