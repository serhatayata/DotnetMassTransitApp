using MassTransit;

namespace Shared.Queue.Contracts;

/// <summary>
/// By setting [ConfigureConsumeTopology(false)] on the DeleteRecord message type, you take control of the topology             configuration. This can be useful in scenarios where you need precise control over how the RabbitMQ exchanges, queues, and  bindings are set up, such as when integrating with existing infrastructure or when applying custom routing logic.
/// </summary>
[ConfigureConsumeTopology(false)]
public record DeleteRecord
{
}
