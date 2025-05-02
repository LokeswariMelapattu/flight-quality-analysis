using CsvHelper;
using CsvHelper.Configuration;
using FlightQualityAnalysis.Domain.Entities;
using FlightQualityAnalysis.Domain.Interfaces;
using System.Globalization;
using System.Text.Json;

namespace FlightQualityAnalysis.Infrastructure;
/// <summary>
/// This class is responsible for reading flight details from a CSV file and providing methods to retrieve the data.
/// It implements the IFlightAnalysisRepository interface.
/// </summary> 
public class FlightAnalysisRepository : IFlightAnalysisRepository
{
    private readonly string _csvPath;

    public FlightAnalysisRepository(IConfiguration config, IHostEnvironment hostEnvironment)
    {
       var _sourceFilePath = config["AppSettings:SourceFilePath"]?.ToString()??"";
        _csvPath = Path.Combine(hostEnvironment.ContentRootPath, _sourceFilePath); 
    }

/// <summary>
/// Asynchronously retrieves flight details from a CSV file.
/// </summary>
/// <returns>A task that represents the asynchronous operation. The task result contains a collection of flight details.</returns>
    public async Task<IEnumerable<FlightDetails>> GetFlightDetailsAsync()
    {
        if (string.IsNullOrEmpty(_csvPath))
        {
            throw new ArgumentException("CSV source file path cannot be null or empty");
        }
         
        using var reader = new StreamReader(_csvPath); 
        var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,// To ensure the first row is treated as headers
            PrepareHeaderForMatch = args => args.Header.ToLower() // Case-insensitive mapping 
        };
        using var csv = new CsvReader(reader, csvConfig);
        var records = new List<FlightDetails>();
        await foreach (var record in csv.GetRecordsAsync<FlightDetails>())
        {
            records.Add(record);
        }
        return records;
    }
     
}
