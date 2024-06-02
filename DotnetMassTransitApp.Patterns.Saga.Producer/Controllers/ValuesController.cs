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
        var orderId = new Guid("05e302da-abac-4e1c-9a4b-a347e7b357f8");
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

    [HttpGet("order-accepted")]
    public async Task<IActionResult> OrderAcceptedMethod()
    {
        var orderId = new Guid("05e302da-abac-4e1c-9a4b-a347e7b357f8");
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new("queue:order-accepted"));

        await endpoint.Send<OrderAccepted>(
        new OrderAccepted()
        {
            OrderId = orderId,
            CompletionTime = TimeSpan.FromSeconds(10)
        });

        return Ok();
    }
}
