namespace Shared.Queue.Requests;

public class OrderStatusResult
{
    public Guid OrderId { get; init; }
    public DateTime Timestamp { get; init; }
    public short StatusCode { get; init; }
    public string StatusText { get; init; }
}