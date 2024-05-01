namespace Shared.Queue.Contracts;

public class SendNotificationOrder
{
    public Guid OrderId { get; init; }
    public string Email { get; set; }
}
