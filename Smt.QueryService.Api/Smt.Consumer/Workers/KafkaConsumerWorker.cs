using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Smt.Consumer.Events;
using Smt.Consumer.Persistence;
using System.Text.Json;

namespace Smt.Consumer.Workers;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly EventRepository _repository;
    private readonly ILogger<KafkaConsumerWorker> _logger;
    private readonly string _bootstrapServers;

    private const string TopicMetrics = "traffic-metrics";
    private const string TopicViolations = "speed-violations";
    private const string TopicWeather = "weather-conditions";
    private const string GroupId = "smt-consumer-group";

    public KafkaConsumerWorker(
        EventRepository repository,
        ILogger<KafkaConsumerWorker> logger,
        string bootstrapServers)
    {
        _repository = repository;
        _logger = logger;
        _bootstrapServers = bootstrapServers;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeLoop(stoppingToken), stoppingToken);
    }

    private async Task ConsumeLoop(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _bootstrapServers,
            GroupId = GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true,
            AllowAutoCreateTopics = true
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(new[] { TopicMetrics, TopicViolations, TopicWeather });

        _logger.LogInformation("KafkaConsumerWorker started — subscribed to {Topics}",
            string.Join(", ", TopicMetrics, TopicViolations, TopicWeather));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                _logger.LogInformation(
                    "Event received from topic {Topic} partition {Partition} offset {Offset}",
                    result.Topic, result.Partition, result.Offset);

                await HandleMessage(result.Topic, result.Message.Value, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error consuming message");
            }
        }

        consumer.Close();
    }

    private async Task HandleMessage(string topic, string value, CancellationToken ct)
    {
        switch (topic)
        {
            case TopicMetrics:
                var metric = JsonSerializer.Deserialize<TrafficMetricEvent>(value);
                if (metric is not null)
                {
                    await _repository.SaveMetricAsync(metric, ct);
                    _logger.LogInformation("TrafficMetric saved — location {LocationId} vehicles {VehicleCount}",
                        metric.LocationId, metric.VehicleCount);
                }
                break;

            case TopicViolations:
                var violation = JsonSerializer.Deserialize<SpeedViolationEvent>(value);
                if (violation is not null)
                {
                    await _repository.SaveViolationAsync(violation, ct);
                    _logger.LogInformation("SpeedViolation saved — plate {Plate} speed {Speed}",
                        violation.Plate, violation.Speed);
                }
                break;

            case TopicWeather:
                var weather = JsonSerializer.Deserialize<WeatherConditionEvent>(value);
                if (weather is not null)
                {
                    await _repository.SaveWeatherAsync(weather, ct);
                    _logger.LogInformation("WeatherCondition saved — location {LocationId} temp {Temperature}",
                        weather.LocationId, weather.Temperature);
                }
                break;
        }
    }
}
