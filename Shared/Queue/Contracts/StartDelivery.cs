using System.Windows.Input;

namespace Shared.Queue.Contracts;

public class StartDelivery : ICommandFilter
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
