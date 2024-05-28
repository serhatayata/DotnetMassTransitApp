using Microsoft.AspNetCore.Mvc;

namespace DotnetMassTransitApp.Observability.Monitoring.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult TestFirst()
    {
        Thread.Sleep(1000);
        return Ok();
    }
}
