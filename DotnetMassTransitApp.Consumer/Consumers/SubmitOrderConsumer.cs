using DotnetMassTransitApp.Consumer.Contracts;
using DotnetMassTransitApp.Consumer.Events;
using DotnetMassTransitApp.Consumer.Models;
using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Consumer.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    private readonly IOrderSubmitter _orderSubmitter;

    public SubmitOrderConsumer(IOrderSubmitter orderSubmitter)
    {
        _orderSubmitter = orderSubmitter;
    }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await _orderSubmitter.Process(context.Message);

        await context.Send(new StartDelivery(context.Message.OrderId, DateTime.UtcNow));

        await context.Publish<OrderSubmitted>(new
        {
            context.Message
        });
    }
}
