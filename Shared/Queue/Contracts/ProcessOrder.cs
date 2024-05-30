namespace Shared.Queue.Contracts;

public class ProcessOrder
{
    public Guid OrderId { get; set; }
    public Guid ProcessingId { get; set; }
}
