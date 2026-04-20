using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smt.Producer.Events;
using Smt.Producer.Producers;

namespace Smt.Producer.Workers;

public class EventGeneratorWorker : BackgroundService
{
    private readonly KafkaEventProducer _producer;
    private readonly ILogger<EventGeneratorWorker> _logger;
    private readonly string[] _locations = { "loc-001", "loc-002", "loc-003" };
    private readonly string[] _plates = { "ABC123", "XYZ789", "DEF456", "GHI101", "JKL202" };
    private readonly Random _random = new();

    private const string TopicMetrics = "traffic-metrics";
    private const string TopicViolations = "speed-violations";
    private const string TopicWeather = "weather-conditions";

    public EventGeneratorWorker(KafkaEventProducer producer, ILogger<EventGeneratorWorker> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EventGeneratorWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            foreach (var location in _locations)
            {
                await PublishTrafficMetric(location, stoppingToken);
                await PublishWeatherCondition(location, stoppingToken);

                if (_random.NextDouble() < 0.3)
                    await PublishSpeedViolation(location, stoppingToken);
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private Task PublishTrafficMetric(string locationId, CancellationToken ct)
    {
        var avgSpeed = _random.Next(35, 85);
        var evt = new TrafficMetricEvent
        {
            LocationId = locationId,
            TimestampUtc = DateTime.UtcNow,
            VehicleCount = _random.Next(200, 1800),
            AvgSpeed = avgSpeed,
            MaxSpeed = avgSpeed + _random.Next(10, 35),
            MinSpeed = Math.Max(5, avgSpeed - _random.Next(10, 25))
        };
        return _producer.PublishAsync(TopicMetrics, locationId, evt, ct);
    }

    private Task PublishWeatherCondition(string locationId, CancellationToken ct)
    {
        var evt = new WeatherConditionEvent
        {
            LocationId = locationId,
            TimestampUtc = DateTime.UtcNow,
            Temperature = Math.Round(_random.NextDouble() * 35, 1),
            Humidity = Math.Round(_random.NextDouble() * 100, 1),
            Rain = _random.NextDouble() < 0.3,
            Snow = _random.NextDouble() < 0.1
        };
        return _producer.PublishAsync(TopicWeather, locationId, evt, ct);
    }

    private Task PublishSpeedViolation(string locationId, CancellationToken ct)
    {
        var limit = 60;
        var speed = limit + _random.Next(10, 50);
        var evt = new SpeedViolationEvent
        {
            ViolationId = Guid.NewGuid().ToString(),
            Plate = _plates[_random.Next(_plates.Length)],
            LocationId = locationId,
            Speed = speed,
            SpeedLimit = limit,
            TimestampUtc = DateTime.UtcNow
        };
        return _producer.PublishAsync(TopicViolations, evt.Plate, evt, ct);
    }
}
