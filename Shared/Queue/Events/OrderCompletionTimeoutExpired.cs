namespace Shared.Queue.Events;

public class OrderCompletionTimeoutExpired
{
    public Guid OrderId { get; set; }
}
