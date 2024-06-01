using MassTransit;
using Shared.Queue.Contracts;
using Shared.Queue.Events;
using Shared.Queue.Saga;

namespace DotnetMassTransitApp.Patterns.Saga.StateMachine.Definitions;

public class OrderStateDefinition
    : SagaDefinition<OrderState>
{
    public OrderStateDefinition()
    {
        // specify the message limit at the endpoint level, which influences
        // the endpoint prefetch count, if supported
        Endpoint(e => e.ConcurrentMessageLimit = 16);
    }

    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator, 
        ISagaConfigurator<OrderState> sagaConfigurator)
    {
        var partition = endpointConfigurator.CreatePartitioner(16);

        sagaConfigurator.Message<SubmitOrder>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
        sagaConfigurator.Message<OrderAccepted>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
        sagaConfigurator.Message<OrderCanceled>(x => x.UsePartitioner(partition, m => m.Message.OrderId));
    }
}
