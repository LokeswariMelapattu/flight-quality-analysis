using FlightQualityAnalysis.Services.DTOs;

namespace FlightQualityAnalysis.Services
{
    /// <summary>
    /// This interface defines the contract for the flight analysis service, which is responsible for analyzing flight data and providing results.
    /// </summary>
    public interface IFlightAnalysisService
    {
        /// <summary>
        /// Asynchronously retrieves flight details from the flight analysis service.   
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of flight details.</returns>
        Task<IEnumerable<FlightDetailsResultDTO>> GetFlightDetailsAsync();

        /// <summary>
        /// Asynchronously analyzes flight sequences and returns the results.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of analysis results.</returns>
        Task<IEnumerable<AnalyzeFlightSequencesResultDTO>> AnalyzeFlightSequencesAsync(); 
    }
}
