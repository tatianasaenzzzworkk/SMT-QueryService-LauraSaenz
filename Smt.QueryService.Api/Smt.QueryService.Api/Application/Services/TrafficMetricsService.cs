using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Smt.QueryService.Api.Application.Dtos;
using Smt.QueryService.Api.Application.Interfaces;
using Smt.QueryService.Api.Infrastructure.Configuration;

namespace Smt.QueryService.Api.Application.Services;

public class TrafficMetricsService : ITrafficMetricsService
{
    private readonly ILogger<TrafficMetricsService> _logger;
    private readonly AppSettings _settings;

    public TrafficMetricsService(
        ILogger<TrafficMetricsService> logger,
        IOptions<AppSettings> options)
    {
        _logger = logger;
        _settings = options.Value;
    }

    public Task<IReadOnlyCollection<TrafficMetricResponseDto>> GetMetricsAsync(
        string locationId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(locationId))
            throw new ArgumentException("locationId es obligatorio.", nameof(locationId));

        if (toUtc <= fromUtc)
            throw new ArgumentException("El rango de fechas es inválido.");

        var results = new List<TrafficMetricResponseDto>();
        var cursor = fromUtc;
        var random = new Random(locationId.GetHashCode() ^ fromUtc.GetHashCode() ^ toUtc.GetHashCode());

        while (cursor < toUtc)
        {
            var vehicleCount = random.Next(200, 1800);
            var avgSpeed = random.Next(35, 85);
            var maxSpeed = avgSpeed + random.Next(10, 35);
            var minSpeed = Math.Max(5, avgSpeed - random.Next(10, 25));

            results.Add(new TrafficMetricResponseDto
            {
                LocationId = locationId,
                TimestampUtc = cursor,
                VehicleCount = vehicleCount,
                AvgSpeed = avgSpeed,
                MaxSpeed = maxSpeed,
                MinSpeed = minSpeed
            });

            cursor = cursor.AddMinutes(_settings.DefaultMetricsIntervalMinutes);
        }

        _logger.LogInformation(
            "Generated {MetricCount} traffic metrics for location {LocationId} between {FromUtc} and {ToUtc}",
            results.Count, locationId, fromUtc, toUtc);

        return Task.FromResult<IReadOnlyCollection<TrafficMetricResponseDto>>(results);
    }
}