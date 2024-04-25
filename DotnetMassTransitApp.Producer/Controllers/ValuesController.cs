using DotnetMassTransitApp.Producer.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace DotnetMassTransitApp.Producer.Controllers;

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

    [HttpGet("send-endpoint")]
    public async Task<IActionResult> SendEndpointMethod()
    {
        try
        {
            string serviceAddress = "rabbitmq://localhost/submit-order-queue";
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri(serviceAddress));
            //var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:input-queue"));

            var timeout = TimeSpan.FromSeconds(5);
            using var source = new CancellationTokenSource(timeout);

            await endpoint.Send(new SubmitOrder
            {
                OrderId = Guid.NewGuid(),
                Sku = "sku-sample",
                Quantity = 10,
                User = "Serhat"
            }, source.Token);

            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }
}
