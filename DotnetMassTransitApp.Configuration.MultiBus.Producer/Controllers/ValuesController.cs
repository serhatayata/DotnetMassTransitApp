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
    private readonly Bind<IThirdBus, IPublishEndpoint> _thirdBusPublishEndpoint;
    private readonly IPublishEndpoint _publishEndpoint;

    public ValuesController(
        Bind<ISecondBus, IPublishEndpoint> secondBusPublishEndpoint, 
        Bind<IThirdBus, IPublishEndpoint> thirdBusPublishEndpoint, 
        IPublishEndpoint publishEndpoint)
    {
        _secondBusPublishEndpoint = secondBusPublishEndpoint;
        _thirdBusPublishEndpoint = thirdBusPublishEndpoint;
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

    [HttpGet("third-bus-publish")]
    public async Task<IActionResult> ThirdBusPublish()
    {
        await _thirdBusPublishEndpoint.Value.Publish<SendNotificationOrder>(
        new SendNotificationOrder()
        {
            OrderId = Guid.NewGuid(),
            Email = "test@test.com"
        });

        return Ok();
    }
}
