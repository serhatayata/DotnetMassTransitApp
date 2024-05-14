using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ValuesController(
        IPublishEndpoint publishEndpoint,
        ISendEndpointProvider sendEndpointProvider)
    {
        _publishEndpoint = publishEndpoint;
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
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(
            new Uri("exchange:change-like-exchange?type=direct&routingKey=change-like-routing-key"));

        await endpoint.Send(
        new ChangeLike
        {
            ProductId = (new Random()).Next(1000, 99999999),
            IsPlus = true
        });

        return Ok();
    }

    [HttpGet("fanout-exchange")]
    public async Task<IActionResult> FanoutExchangeMethod()
    {
        await _publishEndpoint.Publish(
        new RefundOrder
        {
            OrderId = Guid.NewGuid()
        });

        return Ok();
    }

    [HttpGet("headers-exchange")]
    public async Task<IActionResult> HeadersExchangeMethod()
    {
        var headers = new Dictionary<string, object>
        {
            { "key1", "value1" },
            { "key2", "value2" }
        };

        await _publishEndpoint.Publish(
        new NotificationSms
        {
            OrderId = Guid.NewGuid()
        },
        context =>
        {
            foreach (var header in headers)
                context.Headers.Set(header.Key, header.Value);
        });

        return Ok();
    }
}
