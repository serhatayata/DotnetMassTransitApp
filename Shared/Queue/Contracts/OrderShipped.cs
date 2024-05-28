namespace Shared.Queue.Contracts;

public class OrderShipped
{
    public Guid OrderId { get; init; }
    public DateTime ShipDate { get; init; }
}
