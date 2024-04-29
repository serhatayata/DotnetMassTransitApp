using MassTransit;
using Shared.Queue.Requests;
using Shared.Queue.Responses;

namespace DotnetMassTransitApp.Consumer.Consumers;

public class FinalizeOrderConsumer : IConsumer<FinalizeOrderRequest>
{
    public async Task Consume(ConsumeContext<FinalizeOrderRequest> context)
    {
        var result = new FinalizeOrderResponse()
        {
            OrderId = Guid.NewGuid(),
            CreationDate = DateTime.Now
        };

        await context.RespondAsync(result);
    }
}
