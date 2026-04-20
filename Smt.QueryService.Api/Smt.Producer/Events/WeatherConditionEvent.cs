namespace Smt.Producer.Events;

public class WeatherConditionEvent
{
    public string LocationId { get; set; } = default!;
    public DateTime TimestampUtc { get; set; }
    public double Temperature { get; set; }
    public double Humidity { get; set; }
    public bool Rain { get; set; }
    public bool Snow { get; set; }
}