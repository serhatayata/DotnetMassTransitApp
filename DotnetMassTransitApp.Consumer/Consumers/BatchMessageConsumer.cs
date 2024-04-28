using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Consumer.Consumers;

public class BatchMessageConsumer : IConsumer<Batch<SubmitOrder>>
{
    public async Task Consume(ConsumeContext<Batch<SubmitOrder>> context)
    {
        for (int i = 0; i < context.Message.Length; i++)
        {
            ConsumeContext<SubmitOrder> message = context.Message[i];
        }
    }
}
