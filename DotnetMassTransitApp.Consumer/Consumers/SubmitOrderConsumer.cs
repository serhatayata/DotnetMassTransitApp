using DotnetMassTransitApp.Consumer.Models;
using MassTransit;
using MassTransit.Clients;
using Shared.Queue.Contracts;
using Shared.Queue.Events;

namespace DotnetMassTransitApp.Consumer.Consumers;

public class SubmitOrderConsumer : IConsumer<SubmitOrder>
{
    private readonly IOrderSubmitter _orderSubmitter;

    public SubmitOrderConsumer(
        IOrderSubmitter orderSubmitter)
    {
        _orderSubmitter = orderSubmitter;
    }

    public async Task Consume(ConsumeContext<SubmitOrder> context)
    {
        await _orderSubmitter.Process(context.Message);

        await context.Send(new StartDelivery(context.Message.OrderId, DateTime.UtcNow));

        await context.Publish<OrderSubmitted>(new
        {
            OrderId = context.Message.OrderId,
            OrderDate = DateTime.Now
        });
    }
}
