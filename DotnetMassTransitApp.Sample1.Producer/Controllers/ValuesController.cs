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
}
