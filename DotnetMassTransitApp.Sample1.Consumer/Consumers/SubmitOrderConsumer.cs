using DotnetMassTransitApp.Sample1.Consumer.Extensions;
using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Consumer.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public Task Consume(ConsumeContext<SubmitOrder> context)
    {
        var routingKey = context.RoutingKey() ?? string.Empty;
        var suffix = routingKey.RoutingKeySuffixByComma();

        switch (suffix)
        {
            case "type-1": 
                Console.WriteLine("Type-1"); break;
            case "type-2":
                Console.WriteLine("Type-2"); break;
            default: break;
        }

        return Task.CompletedTask;
    }
}
