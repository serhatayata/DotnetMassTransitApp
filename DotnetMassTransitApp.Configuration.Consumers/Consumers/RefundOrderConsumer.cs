using MassTransit;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Consumers.Consumers;

public class RefundOrderConsumer : IJobConsumer<RefundOrder>
{
    public Task Run(JobContext<RefundOrder> context)
    {
        return Task.CompletedTask;
    }
}
