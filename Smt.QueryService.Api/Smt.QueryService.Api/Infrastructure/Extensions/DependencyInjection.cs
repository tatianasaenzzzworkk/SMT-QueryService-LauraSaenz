using Smt.QueryService.Api.Application.Interfaces;
using Smt.QueryService.Api.Application.Services;

namespace Smt.QueryService.Api.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ITrafficMetricsService, TrafficMetricsService>();
        return services;
    }
}