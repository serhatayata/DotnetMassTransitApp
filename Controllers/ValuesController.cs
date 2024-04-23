using DotnetMassTransitApp.Commands;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace DotnetMassTransitApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ValuesController(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpGet("message-correlation")]
    public async Task<IActionResult> MessageCorrelationMethod()
    {
        var orderId = Guid.NewGuid();
        //MessageCorrelation.UseCorrelationId<SubmitOrder>(x => x.OrderId);

        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:submit-order"));

        // 1
        //await endpoint.Send<SubmitOrder>(new { OrderId = InVar.Id }, sendContext =>
        //    sendContext.CorrelationId = orderId);

        // 2
        //await _sendEndpoint.Send<SubmitOrder>(new
        //{
        //    OrderId = orderId,
        //    __CorrelationId = orderId
        //});

        // 3

        await endpoint.Send<SubmitOrder>(new 
        { 
            OrderId = InVar.Id,
            Sku = "Test_sku",
            Quantity = 10,
            User = "Serhat"
        });

        return Ok();
    }
}