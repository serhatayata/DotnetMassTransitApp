namespace Shared.Queue.Contracts;

public record OrderItem
{
    public Guid OrderId { get; init; }
    public string ItemNumber { get; init; }
}
