using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Queue.Contracts;

namespace DotnetMassTransitApp.Configuration.Topology.Controllers;

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

    [HttpGet("reformat-harddrive-publish")]
    public async Task<IActionResult> ReformatHardDrivePublishMethod()
    {
        await _publishEndpoint.Publish<ICommand>(
        new ReformatHardDrive
        {
            ProductId = (new Random()).Next(9999, 9999999)
        });

        return Ok();
    }
}
