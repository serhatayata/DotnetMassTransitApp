using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Responses;

namespace DotnetMassTransitApp.Requests.Consumers;

/// <summary>
/// If the OrderId is found in the repository, an OrderStatusResult message will be sent to the response address included with  the request. The waiting request client will handle the response and complete the returned Task allowing the requesting     application to continue.
/// If the OrderId was not found, the consumer throws an exception.MassTransit catches the exception, generates a               Fault<CheckOrderStatus> message, and sends it to the response address.The request client handles the fault message and      throws a RequestFaultException via the awaited Task containing the exception detail.
/// </summary>
public class CheckOrderStatusConsumer : IConsumer<CheckOrderStatus>
{
    public async Task Consume(ConsumeContext<CheckOrderStatus> context)
    {
        var notFound = true;

        if (notFound)
            await context.RespondAsync<OrderNotFound>(context.Message);
        else
            await context.RespondAsync<OrderStatusResult>(
            new OrderStatusResult
            {
                OrderId = context.Message.OrderId,
                StatusCode = 200,
                Timestamp = DateTime.UtcNow,
                StatusText = "Status 200 text"
            });
    }
}
