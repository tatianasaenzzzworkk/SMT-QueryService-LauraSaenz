using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Prometheus;
using Serilog;
using Serilog.Formatting.Json;
using Smt.QueryService.Api.Application.Interfaces;
using Smt.QueryService.Api.Infrastructure.Configuration;
using Smt.QueryService.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env");
if (File.Exists(envFile))
{
    DotNetEnv.Env.Load(envFile);
}

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("ServiceName", builder.Configuration["App:ServiceName"] ?? "smt-query-service")
    .Enrich.WithProperty("Environment", builder.Configuration["App:EnvironmentName"] ?? "Development")
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(AppSettings.SectionName));

builder.Services.AddApplicationServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoConnection = builder.Configuration["Mongo__ConnectionString"] ?? "mongodb://smt:smt123@smt-mongodb:27017";
var kafkaBootstrap = builder.Configuration["Kafka__BootstrapServers"] ?? "kafka:29092";

builder.Services
    .AddHealthChecks()
    .AddMongoDb(
        sp => new MongoDB.Driver.MongoClient(mongoConnection),
        name: "mongodb",
        tags: new[] { "db", "nosql" })
    .AddKafka(
        setup =>
        {
            setup.BootstrapServers = kafkaBootstrap;
        },
        name: "kafka",
        tags: new[] { "broker", "messaging" });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Prometheus
app.UseHttpMetrics();
app.MapMetrics("/metrics/system");

app.MapControllers();
app.MapHealthChecks("/health");

app.MapHealthChecks("/health/detail", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.Run();

public partial class Program { }