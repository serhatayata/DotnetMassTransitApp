namespace Shared.Queue.Events;

public class OrderSubmitted
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
}
