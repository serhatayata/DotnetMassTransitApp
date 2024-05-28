using MassTransit;

namespace Shared.Queue.Saga.Contracts;

public record CompleteOrder : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; }
    public DateTime OrderDate { get; init; }
}
