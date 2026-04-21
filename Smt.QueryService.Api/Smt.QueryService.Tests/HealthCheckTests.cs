using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Smt.QueryService.Tests;

public class HealthCheckTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthCheckTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOk()
    {
        var response = await _client.GetAsync("/health-check");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Metrics_WithValidParams_ReturnsOk()
    {
        var response = await _client.GetAsync(
            "/metrics?locationId=loc-001&from=2026-04-01T00:00:00Z&to=2026-04-02T00:00:00Z");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}