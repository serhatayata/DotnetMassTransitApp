using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Consumer.Models;

public class OrderSubmitter : IOrderSubmitter
{
    public async Task Process(SubmitOrder order)
    {
        // Logic to process the submitted order
        Console.WriteLine($"Processing order {order.OrderId}...");
        await Task.Delay(1000); // Simulating processing time
        Console.WriteLine($"Order {order.OrderId} processed.");
    }
}
