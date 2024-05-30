using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Events;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine;

public class ProcessOrderConsumer : IConsumer<ProcessOrder>
{
    public async Task Consume(ConsumeContext<ProcessOrder> context)
    {
        await context.RespondAsync(new OrderProcessed()
        {
            OrderId = context.Message.OrderId, 
            ProcessingId = context.Message.ProcessingId
        });
    }
}
