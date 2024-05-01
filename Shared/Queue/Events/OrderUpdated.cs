namespace Shared.Queue.Events;

public class OrderUpdated
{
    public Guid CorrelationId { get; init; }
    public DateTime Timestamp { get; init; }
    public Guid OrderId { get; init; }
    public Customer Customer { get; init; }

    public async Task<Customer> LoadCustomer(Guid orderId)
    {
        return await Task.FromResult(new Customer()
        {
            OrderId = orderId,
            UserId = Guid.NewGuid(),
            Firstname = "Testa",
            Surname = "Testb"
        });
    }
}

public class Customer
{
    public Guid UserId { get; set; }
    public string Firstname { get; set; }
    public string Surname { get; set; }
    public Guid OrderId { get; set; }
}