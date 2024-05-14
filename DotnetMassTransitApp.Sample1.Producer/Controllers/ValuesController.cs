using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpoint _sendEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ValuesController(
        IPublishEndpoint publishEndpoint,
        ISendEndpoint sendEndpoint,
        ISendEndpointProvider sendEndpointProvider)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpoint = sendEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
    }

    [HttpGet("topic-exchange")]
    public async Task<IActionResult> TopicExchangeMethod()
    {
        await _publishEndpoint.Publish(
        new SubmitOrder
        {
            OrderId = Guid.NewGuid()
        });

        return Ok();
    }

    [HttpGet("topic-exchange-routing-key")]
    public async Task<IActionResult> TopicExchangeRoutingKeyMethod()
    {
        await _publishEndpoint.Publish(
        new SubmitOrder
        {
            OrderId = Guid.NewGuid()
        },
        p =>
        {
            p.SetRoutingKey("sample1-routing-submit-order.type-1");
        });

        return Ok();
    }

    [HttpGet("direct-exchange")]
    public async Task<IActionResult> DirectExchangeMethod()
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:change-like-exchange?type=direct"));

        await endpoint.Send(
        new ChangeLike
        {
            ProductId = (new Random()).Next(1000, 99999999),
            IsPlus = true
        });

        return Ok();
    }
}
