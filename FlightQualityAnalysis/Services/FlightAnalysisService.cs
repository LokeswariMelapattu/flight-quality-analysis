using AutoMapper;
using FlightQualityAnalysis.Domain.Entities;
using FlightQualityAnalysis.Domain.Interfaces;
using FlightQualityAnalysis.Services.DTOs;
using Moq;
using Xunit;

namespace FlightQualityAnalysis.Services;
/// <summary>
/// The FlightAnalysisService implements IFlightAnalysisService interface and provides methods to retrieve flight details and analyze flight sequences.
/// </summary>
/// <remarks>
/// This Service provides methods to retrieve flight details and analyse the flight sequence inconsistencies.
/// It uses the IFlightAnalysisRepository to access the data layer.
/// It uses AutoMapper to map between the entity model (FlightDetails) and the service DTOs (FlightDetailsResultDTO, AnalyzeFlightSequencesResultDTO).
/// It also uses ILogger to log information about the analysis process. 
/// </remarks>
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
        _logger = logger;
    }
    /// <summary>
    /// This method retrieves all flight details from the source file.
    /// </summary>
    /// <returns>List of flight details.</returns>
    /// <remarks>
    /// The method uses the IFlightAnalysisRepository to get the flight details from the data source.
    /// It then maps the entity model (FlightDetails) to the service DTO (FlightDetailsResultDTO) using AutoMapper and returns the result.
    /// </remarks>
    public async Task<IEnumerable<FlightDetailsResultDTO>> GetFlightDetailsAsync()
    {
        var flightDetails = await _flightAnalysisRepository.GetFlightDetailsAsync();
        return _mapper.Map<IEnumerable<FlightDetailsResultDTO>>(flightDetails);
    }

    /// <summary>
    /// This method retrieves flight details and checks for inconsistencies in the flight data. 
    /// </summary>
    /// <returns>List of flight details with inconsistencies.</returns>
    /// <remarks>
    /// The method checks for the following inconsistencies for each flight:
    /// 1. Departure airport is different from the last record's arrival airport.
    /// 2. Departure date time is before the last record's arrival date time.
    /// If any inconsistencies are found, they are added to the result list.
    /// </remarks>
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
            if (lastFlight?.FlightNumber == flightInfo.FlightNumber &&
                (!string.Equals(lastFlight?.ArrivalAirport, flightInfo.DepartureAirport, StringComparison.OrdinalIgnoreCase) || lastFlight?.ArrivalDateTime > flightInfo.DepartureDateTime))
            {
                var reason = "";
                if (lastFlight?.ArrivalDateTime > flightInfo.DepartureDateTime) // for daparture time 
                    reason = $"Flight {flightInfo.FlightNumber} has inconsistent route. " +
                       $"Expected departure: {lastFlight?.ArrivalAirport} after {lastFlight?.ArrivalDateTime}, " +
                       $"but was {flightInfo.DepartureAirport} at {flightInfo.DepartureDateTime}.";
                else // for airport 
                    reason = $"Flight {flightInfo.FlightNumber} has inconsistent route. Expected departure: {lastFlight.ArrivalAirport}, but was {flightInfo.DepartureAirport}";


                // Add inconsistency reason to the identified recoord
                flightInfo.InconsistentReason = reason;
                inconsistencyFlights.Add(flightInfo);
            }
            // Update the last flight record for the next loop iteration
            lastFlight = flightInfo;
        }

        _logger.LogInformation("Flight sequence analysis completed"); 
        return inconsistencyFlights;
    }

}
