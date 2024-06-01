using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Patterns.Saga.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ValuesController(
        ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpGet("submit-order")]
    public async Task<IActionResult> SubmitOrderMethod()
    {
        var orderId = Guid.NewGuid();
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:submit-order"));

        await endpoint.Send<SubmitOrder>(
        new SubmitOrder()
        {
            OrderId = orderId,
            OrderDate = DateTime.Now,
            OrderAmount = 100,
            OrderNumber = "123123",
            OrderItems = new OrderItem[]
            {
               new()
               {
                   OrderId = orderId,
                   ItemNumber = "1123"
               }
            }
        });

        return Ok();            
    }
}
