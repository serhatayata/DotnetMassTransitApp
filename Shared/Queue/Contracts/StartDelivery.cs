namespace Shared.Queue.Contracts;

public class StartDelivery
{
    public StartDelivery()
    {
    }

    public StartDelivery(string orderId, DateTime creationTime)
    {
        this.OrderId = orderId;
        this.CreationTime = creationTime;
    }

    public string OrderId { get; set; }
    public DateTime CreationTime { get; set; }
}
