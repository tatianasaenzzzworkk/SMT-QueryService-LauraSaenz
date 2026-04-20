using MongoDB.Driver;
using Smt.Consumer.Events;

namespace Smt.Consumer.Persistence;

public class EventRepository
{
    private readonly IMongoCollection<TrafficMetricEvent> _metrics;
    private readonly IMongoCollection<SpeedViolationEvent> _violations;
    private readonly IMongoCollection<WeatherConditionEvent> _weather;

    public EventRepository(MongoDbContext context)
    {
        _metrics = context.GetCollection<TrafficMetricEvent>("traffic_metrics");
        _violations = context.GetCollection<SpeedViolationEvent>("speed_violations");
        _weather = context.GetCollection<WeatherConditionEvent>("weather_conditions");
    }

    public Task SaveMetricAsync(TrafficMetricEvent evt, CancellationToken ct = default) =>
        _metrics.InsertOneAsync(evt, cancellationToken: ct);

    public Task SaveViolationAsync(SpeedViolationEvent evt, CancellationToken ct = default) =>
        _violations.InsertOneAsync(evt, cancellationToken: ct);

    public Task SaveWeatherAsync(WeatherConditionEvent evt, CancellationToken ct = default) =>
        _weather.InsertOneAsync(evt, cancellationToken: ct);
}
