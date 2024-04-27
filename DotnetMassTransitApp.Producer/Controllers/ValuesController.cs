using DotnetMassTransitApp.Producer.Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace DotnetMassTransitApp.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly IBus _bus;

    public ValuesController(
        ISendEndpointProvider sendEndpointProvider,
        IBus bus)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _bus = bus;
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

    [HttpGet("address-conventions")]
    public async Task<IActionResult> AddressConventionsMethod()
    {
        try
        {
            var timeout = TimeSpan.FromSeconds(5);
            using var source = new CancellationTokenSource(timeout);

            await _sendEndpointProvider.Send(
            new SubmitOrder
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

    [HttpGet("address-conventions-bus")]
    public async Task<IActionResult> AddressConventionsBusMethod()
    {
        try
        {
            var timeout = TimeSpan.FromSeconds(5);
            using var source = new CancellationTokenSource(timeout);

            await _bus.Send(
            new SubmitOrder
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
