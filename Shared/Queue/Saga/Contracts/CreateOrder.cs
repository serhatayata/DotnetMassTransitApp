using MassTransit;

namespace Shared.Queue.Saga.Contracts;

public record CreateOrder : CorrelatedBy<Guid>
{
    public Guid CorrelationId { get; set; }
}
