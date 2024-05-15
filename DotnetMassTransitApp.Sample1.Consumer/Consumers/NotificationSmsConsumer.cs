using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Consumer.Consumers;

public class NotificationSmsConsumer : IConsumer<NotificationSms>
{
    public Task Consume(ConsumeContext<NotificationSms> context)
    {
        var key1Exists = context.Headers.TryGetHeader("key1", out var data1);
        var key2Exists = context.Headers.TryGetHeader("key2", out var data2);
        return Task.CompletedTask;
    }
}
