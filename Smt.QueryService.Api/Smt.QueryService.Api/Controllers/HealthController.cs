using Microsoft.AspNetCore.Mvc;

namespace Smt.QueryService.Api.Controllers;

[ApiController]
[Route("")]
public class HealthController : ControllerBase
{
    [HttpGet("health-check")]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "ok",
            timestampUtc = DateTime.UtcNow
        });
    }
}