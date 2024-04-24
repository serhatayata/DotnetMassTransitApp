namespace DotnetMassTransitApp.Consumer.Contracts;

public class SubmitOrder
{
    public Guid OrderId { get; set; }
    public string User { get; init; }
    public string Sku { get; init; }
    public int Quantity { get; init; }
}
