using FlightQualityAnalysis.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlightQualityAnalysis.Controllers;

/// <summary>
/// Controller for flight quality analysis operations.
/// This controller provides endpoints for retrieving flight details and analyzing flight sequences.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class FlightQualityAnalysisController : ControllerBase
{
    private readonly IFlightAnalysisService _flightAnalysisService;

    public FlightQualityAnalysisController(IFlightAnalysisService flightAnalysisService)
    {
        _flightAnalysisService = flightAnalysisService;
    }
     /// <summary>
     /// Retrieves flight details from the flight analysis service.
     /// </summary>
     /// <returns>A task that represents the asynchronous operation. The task result contains the flight details.</returns>
    [HttpGet("FlightDetails")]  
    public async Task<IActionResult> GetFlightDetails() => Ok(await _flightAnalysisService.GetFlightDetailsAsync());
     
     /// <summary>
     /// Analyzes flight sequences and returns the results.
     /// </summary>
     /// <returns>A task that represents the asynchronous operation. The task result contains the analysis results.</returns>
    [HttpGet("AnalyzeFlightSequences")]
    public async Task<IActionResult> AnalyzeFlightSequences() => Ok(await _flightAnalysisService.AnalyzeFlightSequencesAsync());

}
