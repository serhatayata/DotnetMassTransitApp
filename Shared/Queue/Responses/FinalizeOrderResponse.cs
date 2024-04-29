namespace Shared.Queue.Responses;

public class FinalizeOrderResponse
{
    public Guid OrderId { get; set; }
    public DateTime CreationDate { get; set; }
}
