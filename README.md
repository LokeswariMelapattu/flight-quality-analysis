# flight-quality-analysis

## Overview
The **Flight Quality Analysis** project provides tools to analyze flight data for inconsistencies in flight sequences. It includes:
- A service (`FlightAnalysisService`) to retrieve and analyze flight data.
- Unit tests to ensure the correctness of the service.

## Features
- Retrieve flight details from a repository.
- Analyze flight sequences for inconsistencies, such as:
  - Departure airport mismatches.
  - Departure times earlier than the previous flight's arrival time.

## Technologies
- **.NET 8**
- **csvHelper** for reading CSV File.
- **xUnit** for unit testing.
- **Moq** for mocking dependencies.
- **AutoMapper** for mapping entities to DTOs.

## Source file format  
Below is a sample of the input CSV data used by the application:

| id  | aircraft_registration_number | aircraft_type | flight_number | departure_airport | departure_datetime | arrival_airport | arrival_datetime |
|-----|-------------------------------|----------------|----------------|--------------------|---------------------|------------------|-------------------|
| 617 | SS-GLC                        | 350            | A284           | JFK                | 6/16/2023 19:17     | DXB              | 6/17/2023 1:01    |
| 885 | HA-ZNR                        | 787            | A284           | DXB                | 8/6/2023 12:48      | LHR              | 8/7/2023 1:30     |

> Place this file inside the `Data` folder as `flights.csv`, and configure the file path in `appsettings.json`.

## Project Structure
root
│
├── Data
│   └── flights.csv
│
├── Infrastructure
│   └── FlightAnalysisRepository.cs
│
├── Domain
│   ├── Entities 
        └── FlightDetails.cs 
│   └── Interfaces
        └── IFlightAnalysisRepository.cs
│
├── Services
│   ├── DTOs 
│   │   ├── FlightDetailsResultDTO.cs
│   │   └── AnalyzeFlightSequencesResultDTO.cs
│   ├── Mappers 
│   │   └── MappingProfile.cs
│   ├── IFlightAnalysisService.cs
│   └── FlightAnalysisService.cs  
│
├── Controllers
│   └── FlightAnalysisController.cs  
│
├── Handlers
│   └── ExceptionHandlingMiddleware.cs 
│
├── Tests
│   └── Services 
│       └── FlightAnalysisServiceTests.cs 
│
├── AppSettings.cs  
├── appsettings.json  
└── Program.cs   

## How to Run Project
1. Clone the repository:
   ```bash
   git clone https://github.com/LokeswariMelapattu/flight-quality-analysis.git

    ```
2. Navigate to project directory:
    ```bash
    cd flight-quality-analysis/FlightQualityAnalaysis 
    dotnet clean
    dotnet build
    dotnet run
    ```
3. Use curl or Postman and call the below endpoints to check the response 
    To retrieve all flight details - 
        http://localhost:5044/api/FlightQualityAnalysis/FlightDetails

    Analyse the flights data and return inconsistent flight details 
        http://localhost:5044/api/FlightQualityAnalysis/AnalyzeFlightSequences


## How to Run Tests
 Navigate to project folder FlightQualityAnalysis and run test command
    
    ```bash  
    dotnet test
    ```
## Test cases  

The following test cases are implemented to ensure correct behavior of the flight data analysis:
 
1. Retrieve All Flight Data
    Ensures that all flight data is correctly retrieved from the CSV file without any loss or modification.

2. No Inconsistencies in Clean Data
    Verifies that no inconsistencies are reported when the source file contains valid and consistent flight sequences.

3. Single Inconsistency in Departure Airport
    Confirms that a single inconsistency is correctly identified when there is a mismatch in the departure airport for a flight sequence.

4. Multiple Inconsistencies in Departure Airports
    Validates that multiple inconsistencies are identified when there are multiple mismatches in the sequences of departure airports.

5. Inconsistencies in Departure DateTime
    Ensures that inconsistencies are reported when a flight's departure time is earlier than the arrival time of the previous flight in the sequence.