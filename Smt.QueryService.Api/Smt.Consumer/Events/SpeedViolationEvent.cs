namespace Smt.Consumer.Events;

public class SpeedViolationEvent
{
    public string ViolationId { get; set; } = default!;
    public string Plate { get; set; } = default!;
    public string LocationId { get; set; } = default!;
    public double Speed { get; set; }
    public double SpeedLimit { get; set; }
    public DateTime TimestampUtc { get; set; }
}
