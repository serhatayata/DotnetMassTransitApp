using Microsoft.AspNetCore.Mvc;

namespace DotnetMassTransitApp.Observability.Metrics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult TestGet()
    {
        Thread.Sleep(1000);
        return Ok();
    }
}
