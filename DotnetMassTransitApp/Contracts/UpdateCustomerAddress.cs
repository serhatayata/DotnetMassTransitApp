namespace DotnetMassTransitApp.Contracts;

/// <summary>
/// Properties with private set; are not recommended as they are not serialized by default when using System.Text.Json.
/// </summary>
public class UpdateCustomerAddress
{
    public Guid CommandId { get; set; }
    public DateTime Timestamp { get; set; }
    public string CustomerId { get; set; }
    public string HouseNumber { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
}
