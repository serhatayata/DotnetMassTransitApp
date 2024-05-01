namespace Shared.Queue.Contracts;

public class StartDelivery
{
    public StartDelivery()
    {
    }

    public StartDelivery(Guid orderId, DateTime creationTime)
    {
        this.OrderId = orderId;
        this.CreationTime = creationTime;
    }

    public Guid OrderId { get; set; }
    public DateTime CreationTime { get; set; }
}
