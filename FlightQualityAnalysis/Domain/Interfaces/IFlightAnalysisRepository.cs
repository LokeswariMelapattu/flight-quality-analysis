using FlightQualityAnalysis.Domain.Entities;

namespace FlightQualityAnalysis.Domain.Interfaces;

/// <summary>
/// This interface defines the contract for the flight analysis repository, which is responsible for retrieving flight details from a data source.
/// </summary>
public interface IFlightAnalysisRepository
{
    /// <summary>
    /// Asynchronously retrieves flight details from a data source.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a collection of flight details.</returns>
    Task<IEnumerable<FlightDetails>> GetFlightDetailsAsync(); 
}
