using MassTransit;

namespace Shared.Queue.Saga.Contracts;

public class OrderInvoiced : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; }
    public DateTime Timestamp { get; init; }
    public decimal Amount { get; init; }
}
