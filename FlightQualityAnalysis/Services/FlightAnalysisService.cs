using AutoMapper;
using FlightQualityAnalysis.Domain.Entities;
using FlightQualityAnalysis.Domain.Interfaces;
using FlightQualityAnalysis.Services.DTOs;
using Moq;
using Xunit;

namespace FlightQualityAnalysis.Services;
/// <summary>
/// The FlightAnalysisService implements IFlightAnalysisService interface
/// And it provides methods to retrieve flight details and analyse the flight sequence inconsistencies.
/// It uses the IFlightAnalysisRepository to access the data layer.
/// </summary>
public class FlightAnalysisService : IFlightAnalysisService
{
    private readonly IFlightAnalysisRepository _flightAnalysisRepository;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;
    public FlightAnalysisService(IFlightAnalysisRepository flightAnalysisRepository, IMapper mapper,
        ILogger<FlightAnalysisService> logger)
    {
        _flightAnalysisRepository = flightAnalysisRepository;
        _mapper = mapper;
        _logger= logger;
    }
    /// <summary>
    /// This method retrieves all flight details from the source file.
    /// </summary>
    public async Task<IEnumerable<FlightDetailsResultDTO>> GetFlightDetailsAsync()
    {
        var flightDetails = await _flightAnalysisRepository.GetFlightDetailsAsync();
        // Auto mapper to convert the entity model(FlightDetails) to service DTOs (FlightDetailsResultDTO)
        return _mapper.Map<IEnumerable<FlightDetailsResultDTO>>(flightDetails);
    }
     
    /// <summary>
    /// This method retrieves flight details and checks for inconsistencies in the flight data.
    /// Returns all flight details which has inconsistencies.
    /// </summary>
    public async Task<IEnumerable<AnalyzeFlightSequencesResultDTO>> AnalyzeFlightSequencesAsync()
    {
        
        var result = await _flightAnalysisRepository.GetFlightDetailsAsync();
        _logger.LogInformation("Flight sequence analysis started");
        // Auto mapper to convert the entity model(FlightDetails) to service DTO (AnalyzeFlightSequencesResultDTO)
        // and filter records to include only the FlightNumbers that has more than one occurance for better performance
        // in foreach loop
        var flightDetails = _mapper.Map<IEnumerable<AnalyzeFlightSequencesResultDTO>>(result)
                            .GroupBy(x => x.FlightNumber)
                            .Where(g => g.Skip(1).Any()) 
                            .SelectMany(g => g)
                            .OrderBy(t => t.FlightNumber)
                            .ThenBy(t => t.DepartureDateTime);

        var inconsistencyFlights = new List<AnalyzeFlightSequencesResultDTO>();
        AnalyzeFlightSequencesResultDTO? lastFlight = null; 
        foreach (var flightInfo in flightDetails)
        {
            // Check for inconsistencies in the flight details
            // 1. Check if the flight number is the same as the last one
            // 2. Check if the departure airport is different from the last record arrival airport
            //      or departure date time is before the last record arrival date time
            if (lastFlight?.FlightNumber == flightInfo.FlightNumber &&
                (lastFlight?.ArrivalAirport != flightInfo.DepartureAirport 
                    || lastFlight?.ArrivalDateTime > flightInfo.DepartureDateTime) )
            {
                var reason = "";
                if (lastFlight?.ArrivalAirport != flightInfo.DepartureAirport)
                    reason = $"Flight {flightInfo.FlightNumber} has inconsistent route. Expected departure: {lastFlight.ArrivalAirport}, but was {flightInfo.DepartureAirport}";
                else // for time comparision
                    reason = $"Flight {flightInfo.FlightNumber} has inconsistent route. " +
                       $"Expected departure: {lastFlight?.ArrivalAirport} after {lastFlight?.ArrivalDateTime}, " +
                       $"but was {flightInfo.DepartureAirport} at {flightInfo.DepartureDateTime}.";

                // Add inconsistency reason to the identified recoord
                flightInfo.InconsistentReason=reason;
                inconsistencyFlights.Add(flightInfo);
            }
            // Update the last flight record for the next loop iteration
            lastFlight = flightInfo; 
        }

        _logger.LogInformation("Flight sequence analysis completed");
        return inconsistencyFlights;
    }
    
}
