using Smt.QueryService.Api.Application.Dtos;

namespace Smt.QueryService.Api.Application.Interfaces;

public interface ITrafficMetricsService
{
    Task<IReadOnlyCollection<TrafficMetricResponseDto>> GetMetricsAsync(
        string locationId,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);
}
