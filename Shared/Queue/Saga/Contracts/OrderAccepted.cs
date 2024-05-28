using MassTransit;

namespace Shared.Queue.Saga.Contracts;

public record OrderAccepted : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; init; }
    public DateTime Timestamp { get; init; }
}
