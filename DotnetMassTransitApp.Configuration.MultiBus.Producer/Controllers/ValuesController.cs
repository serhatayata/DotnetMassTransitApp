using MassTransit.DependencyInjection;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Buses;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.MultiBus.Producer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly Bind<ISecondBus, IPublishEndpoint> _secondBusPublishEndpoint;
    private readonly IPublishEndpoint _publishEndpoint;

    public ValuesController(
        Bind<ISecondBus, IPublishEndpoint> secondBusPublishEndpoint, 
        IPublishEndpoint publishEndpoint)
    {
        _secondBusPublishEndpoint = secondBusPublishEndpoint;
        _publishEndpoint = publishEndpoint;
    }

    [HttpGet("publish")]
    public async Task<IActionResult> Publish()
    {
        await _publishEndpoint.Publish<SubmitOrder>(
        new SubmitOrder()
        {
            OrderId = Guid.NewGuid(),
            OrderAmount = 100,
            OrderDate = DateTime.Now,
            OrderItems = null,
            OrderNumber = "123123"
        });

        return Ok();
    }

    [HttpGet("second-bus-publish")]
    public async Task<IActionResult> SecondBusPublish()
    {
        await _secondBusPublishEndpoint.Value.Publish<RefundOrder>(
        new RefundOrder()
        {
            OrderId = Guid.NewGuid()
        });

        return Ok();
    }
}
