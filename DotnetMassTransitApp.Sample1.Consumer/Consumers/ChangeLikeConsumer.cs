using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Consumer.Consumers;

public class ChangeLikeConsumer : IConsumer<ChangeLike>
{
    public Task Consume(ConsumeContext<ChangeLike> context)
    {
        return Task.CompletedTask;
    }
}
