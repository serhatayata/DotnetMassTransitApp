namespace Shared.Queue.Contracts;

public class SubmitOrder
{
    public Guid OrderId { get; init; }
    public DateTime OrderDate { get; init; }
    public string OrderNumber { get; init; }
    public decimal OrderAmount { get; init; }
    public OrderItem[] OrderItems { get; init; }
}
