using Microsoft.AspNetCore.Mvc;
using Serilog;
using Smt.QueryService.Api.Application.Interfaces;
using Smt.QueryService.Api.Application.Services;
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

builder.Host.UseSerilog((context, config) =>
{
    config
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("ServiceName", context.Configuration["App:ServiceName"] ?? "smt-query-service")
        .Enrich.WithProperty("Environment", context.Configuration["App:EnvironmentName"] ?? "Development")
        .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter());
});

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection(AppSettings.SectionName));

builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddHealthChecks();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
