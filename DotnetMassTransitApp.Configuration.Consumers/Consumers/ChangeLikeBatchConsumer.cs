using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Consumers.Consumers;

public class ChangeLikeBatchConsumer : IConsumer<Batch<ChangeLike>>
{
    public Task Consume(ConsumeContext<Batch<ChangeLike>> context)
    {
        for (int i = 0; i < context.Message.Length; i++)
        {
            ConsumeContext<ChangeLike> message = context.Message[i];
        }

        return Task.CompletedTask;
    }
}
