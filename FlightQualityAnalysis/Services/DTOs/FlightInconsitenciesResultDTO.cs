namespace FlightQualityAnalysis.Services.DTOs;

public class AnalyzeFlightSequencesResultDTO
{
    public int Id { get; set; }
    public string AircraftRegistrationNumber { get; set; } = string.Empty;
    public string AircraftType { get; set; } = string.Empty;
    public string FlightNumber { get; set; } = string.Empty;
    public string DepartureAirport { get; set; } = string.Empty;
    public DateTime DepartureDateTime { get; set; }
    public string ArrivalAirport { get; set; } = string.Empty;
    public DateTime ArrivalDateTime { get; set; } 
    public string InconsistentReason { get; set; } = string.Empty; 
}