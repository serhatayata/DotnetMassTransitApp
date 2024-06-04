using MassTransit;

namespace Shared.Queue.Saga;

public class OrderState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public int CurrentState { get; set; }

    public DateTime OrderDate { get; set; }

    public string? OrderNumber { get; set; }

    public int ReadyEventStatus { get; set; }

    public Guid? ProcessingId { get; set; }
    //public Guid? ProcessOrderRequestId { get; set; }

    public Guid? RequestId { get; set; }
    public Uri? ResponseAddress { get; set; }
    public Guid OrderId { get; set; }

    //public Guid? OrderCompletionTimeoutTokenId { get; set; }

    //public byte[] RowVersion { get; set; }
}
