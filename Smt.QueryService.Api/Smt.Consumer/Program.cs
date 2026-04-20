using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Smt.Consumer.Persistence;
using Smt.Consumer.Workers;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .ConfigureServices((context, services) =>
    {
        var bootstrapServers = context.Configuration["Kafka__BootstrapServers"] ?? "kafka:29092";
        var mongoConnection = context.Configuration["Mongo__ConnectionString"] ?? "mongodb://smt:smt123@smt-mongodb:27017";
        var mongoDatabase = context.Configuration["Mongo__DatabaseName"] ?? "smt_db";

        var mongoContext = new MongoDbContext(mongoConnection, mongoDatabase);
        var repository = new EventRepository(mongoContext);

        services.AddSingleton(mongoContext);
        services.AddSingleton(repository);

        services.AddHostedService(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<KafkaConsumerWorker>>();
            return new KafkaConsumerWorker(repository, logger, bootstrapServers);
        });
    })
    .Build();

await host.RunAsync();
