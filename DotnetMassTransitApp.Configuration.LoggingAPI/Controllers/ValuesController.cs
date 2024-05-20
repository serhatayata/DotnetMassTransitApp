using Microsoft.AspNetCore.Mvc;

namespace DotnetMassTransitApp.Configuration.LoggingAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GenerateError()
    {
        throw new NullReferenceException("null test");

        return Ok();
    }
}
