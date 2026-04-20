namespace Smt.QueryService.Api.Infrastructure.Configuration;

public class AppSettings
{
    public const string SectionName = "App";

    public string ServiceName { get; set; } = "smt-query-service";
    public string EnvironmentName { get; set; } = "Development";
    public int DefaultMetricsIntervalMinutes { get; set; } = 60;
}