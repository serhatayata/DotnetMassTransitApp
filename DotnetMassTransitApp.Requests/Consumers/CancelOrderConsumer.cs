using MassTransit;
using MassTransit.Transports;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Responses;

namespace DotnetMassTransitApp.Requests.Consumers;
using OrderNotFound = Shared.Queue.Responses.OrderNotFound;

public class CancelOrderConsumer : IConsumer<CancelOrder>
{
    public async Task Consume(ConsumeContext<CancelOrder> context)
    {
        var notFound = true;
        var hasShipped = true;

        if (notFound)
        {
            await context.RespondAsync<OrderNotFound>(new { context.Message.OrderId });
            return;
        }

        if (hasShipped)
        {
            var shipDate = DateTime.Now.AddMonths(-1);
            if (context.IsResponseAccepted<OrderAlreadyShipped>())
            {
                await context.RespondAsync<OrderAlreadyShipped>(new { context.Message.OrderId, shipDate });
                return;
            }
            else
                throw new InvalidOperationException("The order has already shipped"); // to throw a RequestFaultException in the client
        }

        // Order cancelled here

        await context.RespondAsync<OrderCanceled>(new { context.Message.OrderId });
    }
}
