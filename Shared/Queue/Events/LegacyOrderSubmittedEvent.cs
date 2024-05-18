using MassTransit;

namespace Shared.Queue.Events;

[EntityName("order-submitted")]
public record LegacyOrderSubmittedEvent
{
}
