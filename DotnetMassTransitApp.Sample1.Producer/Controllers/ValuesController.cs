using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Sample1.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public ValuesController(
        IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
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
}
