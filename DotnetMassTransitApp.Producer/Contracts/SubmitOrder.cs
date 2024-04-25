namespace DotnetMassTransitApp.Producer.Contracts;

public class SubmitOrder
{
    public Guid OrderId { get; set; }
    public string Sku { get; init; }
    public int Quantity { get; init; }
    public string User { get; init; }
}
