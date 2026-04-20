namespace Smt.QueryService.Api.Domain.Models;

public class TrafficMetric
{
    public string LocationId { get; set; } = default!;
    public DateTime TimestampUtc { get; set; }
    public int VehicleCount { get; set; }
    public double AvgSpeed { get; set; }
    public double MaxSpeed { get; set; }
    public double MinSpeed { get; set; }
}