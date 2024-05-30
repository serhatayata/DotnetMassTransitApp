namespace Shared.Queue.Events;

public class OrderProcessed
{
    public Guid OrderId { get; set; }
    public Guid ProcessingId { get; set; }
}
