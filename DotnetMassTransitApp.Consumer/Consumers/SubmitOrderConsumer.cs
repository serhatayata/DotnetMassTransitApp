using DotnetMassTransitApp.Consumer.Contracts;
using DotnetMassTransitApp.Consumer.Events;
using MassTransit;

namespace DotnetMassTransitApp.Consumer.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await context.Publish<OrderSubmitted>(new
        {
            context.Message
        });
    }
}
