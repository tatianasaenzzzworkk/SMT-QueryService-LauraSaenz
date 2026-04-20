using Confluent.Kafka;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Smt.Producer.Producers;

public class KafkaEventProducer : IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventProducer> _logger;

    public KafkaEventProducer(string bootstrapServers, ILogger<KafkaEventProducer> logger)
    {
        _logger = logger;
        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.Leader
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, string key, T @event, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = JsonSerializer.Serialize(@event)
        };

        var result = await _producer.ProduceAsync(topic, message, cancellationToken);

        _logger.LogInformation(
            "Event published to topic {Topic} partition {Partition} offset {Offset}",
            result.Topic, result.Partition, result.Offset);
    }

    public void Dispose() => _producer.Dispose();
}
