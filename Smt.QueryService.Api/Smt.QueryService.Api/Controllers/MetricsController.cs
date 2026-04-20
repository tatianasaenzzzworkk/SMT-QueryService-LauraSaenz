using Microsoft.AspNetCore.Mvc;
using Smt.QueryService.Api.Application.Interfaces;

namespace Smt.QueryService.Api.Controllers;

[ApiController]
[Route("metrics")]
public class MetricsController : ControllerBase
{
    private readonly ITrafficMetricsService _trafficMetricsService;
    private readonly ILogger<MetricsController> _logger;

    public MetricsController(
        ITrafficMetricsService trafficMetricsService,
        ILogger<MetricsController> logger)
    {
        _trafficMetricsService = trafficMetricsService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetMetrics(
        [FromQuery] string locationId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received metrics query for location {LocationId} from {From} to {To}",
            locationId, from, to);

        var result = await _trafficMetricsService.GetMetricsAsync(
            locationId,
            from.ToUniversalTime(),
            to.ToUniversalTime(),
            cancellationToken);

        return Ok(result);
    }
}