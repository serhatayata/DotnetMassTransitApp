namespace DotnetMassTransitApp.Contracts;

public record SubmitOrder : CoreEvent
{
    public Guid OrderId { get; set; }
    public string Sku { get; init; }
    public int Quantity { get; init; }
}
