namespace FlightQualityAnalysis.Domain.Entities;

public class FlightDetails
{
    public int Id { get; set; }
    public string Aircraft_Registration_Number { get; set; } = string.Empty;
    public string Aircraft_Type { get; set; } = string.Empty;
    public string Flight_Number { get; set; } = string.Empty;
    public string Departure_Airport { get; set; } = string.Empty;
    public DateTime Departure_DateTime { get; set; }
    public string Arrival_Airport { get; set; } = string.Empty;
    public DateTime Arrival_DateTime { get; set; } 
     
}