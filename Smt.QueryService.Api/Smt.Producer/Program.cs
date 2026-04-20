using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Smt.Producer.Producers;
using Smt.Producer.Workers;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        var bootstrapServers = context.Configuration["Kafka__BootstrapServers"] ?? "kafka:29092";

        services.AddSingleton(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<KafkaEventProducer>>();
            return new KafkaEventProducer(bootstrapServers, logger);
        });

        services.AddHostedService<EventGeneratorWorker>();
    })
    .Build();

await host.RunAsync();
