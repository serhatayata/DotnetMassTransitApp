using MassTransit;

namespace Shared.Queue.Saga;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }
}
