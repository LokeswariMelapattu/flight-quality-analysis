using FlightQualityAnalysis.Domain.Entities;
using FlightQualityAnalysis.Domain.Interfaces;
using FlightQualityAnalysis.Services;
using Xunit;
using Moq;
using AutoMapper;
using FlightQualityAnalysis.Services.Mapper;

namespace FlightQualityAnalysis.Tests.Services;

/// <summary>
/// This class contains unit tests for the FlightAnalysisService class.
/// It tests the functionality of the service methods, including retrieving flight details and analyzing flight sequences for inconsistencies.
/// </summary>
public class FlightAnalysisServiceTests
{

    private readonly IMapper _mapper;
    private readonly Mock<ILogger<FlightAnalysisService>> _logger;

    public FlightAnalysisServiceTests()
    {
        // Initialize AutoMapper configuration and create a mapper instance
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = config.CreateMapper(); 
        //Initialize ILogger configuration
        _logger = new Mock<ILogger<FlightAnalysisService>>();
    }

    /// <summary>
    /// This test verifies that the GetFlightDetailsAsync method returns all flight details correctly.
    /// It checks that the returned list is not null and contains the expected number of flight details.
    /// It also verifies that the flight number and departure/arrival airports are as expected.
    /// </summary>
    [Fact]
    public async Task GetFlightDetailsAsync__ReturnsAllFlights()
    {
        //Arrange 
        var flightDetails = new List<FlightDetails>
        {
            new FlightDetails
            {
                Id = 1,
                Aircraft_Registration_Number = "ABC123",
                Aircraft_Type = "Boeing 737",
                Flight_Number = "FL123",
                Departure_Airport = "JFK",
                Departure_DateTime = DateTime.Now.AddHours(-2),
                Arrival_Airport = "LAX",
                Arrival_DateTime = DateTime.Now.AddHours(2)
            },
            new FlightDetails
            {
                Id = 2,
                Aircraft_Registration_Number = "XYZ456",
                Aircraft_Type = "Airbus A320",
                Flight_Number = "FL456",
                Departure_Airport = "LAX",
                Departure_DateTime = DateTime.Now.AddHours(-3),
                Arrival_Airport = "ORD",
                Arrival_DateTime = DateTime.Now.AddHours(1)
            }
        };

        var repositoryMock = new Mock<IFlightAnalysisRepository>();
        repositoryMock.Setup(repo => repo.GetFlightDetailsAsync()).ReturnsAsync(flightDetails);

        //Act
        var flightAnalysisService = new FlightAnalysisService(repositoryMock.Object, _mapper, _logger.Object);
        var result = await flightAnalysisService.GetFlightDetailsAsync();

        //Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count());
        Assert.Equal("FL123", resultList[0].FlightNumber);
        Assert.Equal("JFK", resultList[0].DepartureAirport);
        Assert.Equal("LAX", resultList[0].ArrivalAirport);
    }

/// <summary>
/// This test verifies that the AnalyzeFlightSequencesAsync method returns no inconsistencies when all flight details are valid.
/// It checks that the returned list is empty.
/// </summary> 
    [Fact]
    public async Task AnalyzeFlightSequencesAsync_ReturnsNoFlightDepartureAirportInconsistencies()
    {
        // Arrange
        var flightDetails = new List<FlightDetails>
            {
                new FlightDetails
                {
                    Id = 1,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "JFK",
                    Departure_DateTime = DateTime.Now.AddHours(-2),
                    Arrival_Airport = "LAX",
                    Arrival_DateTime = DateTime.Now.AddHours(2)
                },
                new FlightDetails
                {
                    Id = 2,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "LAX",
                    Departure_DateTime = DateTime.Now.AddHours(3),
                    Arrival_Airport = "ORD",
                    Arrival_DateTime = DateTime.Now.AddHours(6)
                },
                new FlightDetails
                {
                    Id = 3,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "ORD",
                    Departure_DateTime = DateTime.Now.AddHours(8),
                    Arrival_Airport = "JFK",
                    Arrival_DateTime = DateTime.Now.AddHours(10)
                }
            };

        var repositoryMock = new Mock<IFlightAnalysisRepository>();
        repositoryMock.Setup(repo => repo.GetFlightDetailsAsync()).ReturnsAsync(flightDetails);
        var flightAnalysisService = new FlightAnalysisService(repositoryMock.Object, _mapper, _logger.Object);

        // Act
        var result = await flightAnalysisService.AnalyzeFlightSequencesAsync();

        // Assert 
        Assert.Empty(result);  
    }

    /// <summary>
    /// This test verifies that the AnalyzeFlightSequencesAsync method returns only one departure airport inconsistency  when other flight details are valid.
    /// It checks that the returned list contains only one inconsistency and verifies the flight number and reason for the inconsistency.
    /// </summary>
    [Fact]
    public async Task AnalyzeFlightSequencesAsync_ReturnsSingleFlightDepartureAirportInconsistencies()
    {
        // Arrange
        var flightDetails = new List<FlightDetails>
            {
                new FlightDetails
                {
                    Id = 1,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "JFK",
                    Departure_DateTime = DateTime.Now.AddHours(-2),
                    Arrival_Airport = "LAX",
                    Arrival_DateTime = DateTime.Now.AddHours(2)
                },
                new FlightDetails
                {
                    Id = 2,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "SFO",
                    Departure_DateTime = DateTime.Now.AddHours(3),
                    Arrival_Airport = "ORD",
                    Arrival_DateTime = DateTime.Now.AddHours(6)
                },
                new FlightDetails
                {
                    Id = 3,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "ORD",
                    Departure_DateTime = DateTime.Now.AddHours(8),
                    Arrival_Airport = "CDG",
                    Arrival_DateTime = DateTime.Now.AddHours(10)
                }
            };
        
        var repositoryMock = new Mock<IFlightAnalysisRepository>();
        repositoryMock.Setup(repo => repo.GetFlightDetailsAsync()).ReturnsAsync(flightDetails);
        var flightAnalysisService = new FlightAnalysisService(repositoryMock.Object, _mapper, _logger.Object);

        // Act
        var result = await flightAnalysisService.AnalyzeFlightSequencesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Single(resultList); // To ensure only one inconsistency
        Assert.Equal("FL123", resultList[0].FlightNumber); 
        Assert.Contains("Expected departure: LAX,", resultList[0].InconsistentReason);
    }

/// <summary>
/// This test verifies that the AnalyzeFlightSequencesAsync method returns multiple inconsistencies when there are multiple flight details with departure airport inconsistencies.
/// It checks that the returned list contains multiple inconsistencies and verifies the flight numbers and reasons for the inconsistencies.
/// </summary>
    [Fact]
    public async Task AnalyzeFlightSequencesAsync_ReturnsMultipleFlightDepartureAirportInconsistencies()
    {
        // Arrange
        var flightDetails = new List<FlightDetails>
            {
                new FlightDetails
                {
                    Id = 1,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "JFK",
                    Departure_DateTime = DateTime.Now.AddHours(-2),
                    Arrival_Airport = "LAX",
                    Arrival_DateTime = DateTime.Now.AddHours(2)
                },
                new FlightDetails
                {
                    Id = 2,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "SFO",
                    Departure_DateTime = DateTime.Now.AddHours(3),
                    Arrival_Airport = "ORD",
                    Arrival_DateTime = DateTime.Now.AddHours(6)
                },
                new FlightDetails
                {
                    Id = 3,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "ORD",
                    Departure_DateTime = DateTime.Now.AddHours(8),
                    Arrival_Airport = "CDG",
                    Arrival_DateTime = DateTime.Now.AddHours(10)
                },

                new FlightDetails
                {
                    Id = 4,
                    Aircraft_Registration_Number = "ABC456",
                    Aircraft_Type = "320",
                    Flight_Number = "FL456",
                    Departure_Airport = "HEL",
                    Departure_DateTime = DateTime.Now.AddHours(-12),
                    Arrival_Airport = "MAS",
                    Arrival_DateTime = DateTime.Now.AddHours(15)
                },
                new FlightDetails
                {
                    Id = 5,
                    Aircraft_Registration_Number = "ABC789",
                    Aircraft_Type = "737",
                    Flight_Number = "FL789",
                    Departure_Airport = "HEL",
                    Departure_DateTime = DateTime.Now.AddHours(-6),
                    Arrival_Airport = "DXB",
                    Arrival_DateTime = DateTime.Now.AddHours(2)
                },
                new FlightDetails
                {
                    Id = 6,
                    Aircraft_Registration_Number = "ABC789",
                    Aircraft_Type = "737",
                    Flight_Number = "FL789",
                    Departure_Airport = "HEL",
                    Departure_DateTime = DateTime.Now.AddHours(8),
                    Arrival_Airport = "DXB",
                    Arrival_DateTime = DateTime.Now.AddHours(10)
                },
                new FlightDetails
                {
                    Id = 7,
                    Aircraft_Registration_Number = "ABC789",
                    Aircraft_Type = "737",
                    Flight_Number = "FL789",
                    Departure_Airport = "DXB",
                    Departure_DateTime = DateTime.Now.AddHours(12),
                    Arrival_Airport = "HEL",
                    Arrival_DateTime = DateTime.Now.AddHours(20)
                } 
            };

        var repositoryMock = new Mock<IFlightAnalysisRepository>();
        repositoryMock.Setup(repo => repo.GetFlightDetailsAsync()).ReturnsAsync(flightDetails);
        var flightAnalysisService = new FlightAnalysisService(repositoryMock.Object, _mapper, _logger.Object);

        // Act
        var result = await flightAnalysisService.AnalyzeFlightSequencesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        var resultList = result.ToList();
        Assert.Equal(2,resultList.Count()); // To ensure only one inconsistency
        Assert.Equal("FL123", resultList[0].FlightNumber);
        Assert.Equal("FL789", resultList[1].FlightNumber);
        Assert.Contains("Expected departure: LAX,", resultList[0].InconsistentReason);
        Assert.Contains("Expected departure: DXB,", resultList[1].InconsistentReason);
    }

/// <summary>
/// This test verifies that the AnalyzeFlightSequencesAsync method returns multiple inconsistencies when there are multiple flight details with datetime inconsistencies.
/// It checks that the returned list contains multiple inconsistencies and verifies the flight numbers and reasons for the inconsistencies.
/// </summary>
    [Fact]
    public async Task AnalyzeFlightSequencesAsync_ReturnsFlightDepartureTimeInconsistencies()
    {
        // Arrange
        var flightDetails = new List<FlightDetails>
            {
                new FlightDetails
                {
                    Id = 1,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "JFK",
                    Departure_DateTime = DateTime.Now.AddHours(-2),
                    Arrival_Airport = "LAX",
                    Arrival_DateTime = DateTime.Now.AddHours(2)
                },
                new FlightDetails
                {
                    Id = 2,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "LAX",
                    Departure_DateTime = DateTime.Now.AddHours(1).AddMinutes(40),
                    Arrival_Airport = "ORD",
                    Arrival_DateTime = DateTime.Now.AddHours(6)
                },
                new FlightDetails
                {
                    Id = 3,
                    Aircraft_Registration_Number = "ABC123",
                    Aircraft_Type = "Boeing 737",
                    Flight_Number = "FL123",
                    Departure_Airport = "ORD",
                    Departure_DateTime = DateTime.Now.AddHours(8),
                    Arrival_Airport = "CDG",
                    Arrival_DateTime = DateTime.Now.AddHours(10)
                },

                new FlightDetails
                {
                    Id = 4,
                    Aircraft_Registration_Number = "ABC456",
                    Aircraft_Type = "320",
                    Flight_Number = "FL456",
                    Departure_Airport = "HEL",
                    Departure_DateTime = DateTime.Now.AddHours(-12),
                    Arrival_Airport = "MAS",
                    Arrival_DateTime = DateTime.Now.AddHours(15)
                },
                new FlightDetails
                {
                    Id = 5,
                    Aircraft_Registration_Number = "ABC789",
                    Aircraft_Type = "737",
                    Flight_Number = "FL789",
                    Departure_Airport = "HEL",
                    Departure_DateTime = DateTime.Now.AddHours(-6),
                    Arrival_Airport = "DXB",
                    Arrival_DateTime = DateTime.Now.AddHours(2)
                }, 
                new FlightDetails
                {
                    Id = 7,
                    Aircraft_Registration_Number = "ABC789",
                    Aircraft_Type = "737",
                    Flight_Number = "FL789",
                    Departure_Airport = "DXB",
                    Departure_DateTime = DateTime.Now.AddHours(1),
                    Arrival_Airport = "HEL",
                    Arrival_DateTime = DateTime.Now.AddHours(9)
                }
            };

        var repositoryMock = new Mock<IFlightAnalysisRepository>();
        repositoryMock.Setup(repo => repo.GetFlightDetailsAsync()).ReturnsAsync(flightDetails);
        var flightAnalysisService = new FlightAnalysisService(repositoryMock.Object, _mapper, _logger.Object);

        // Act
        var result = await flightAnalysisService.AnalyzeFlightSequencesAsync();

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count()); // To ensure only one inconsistency
        Assert.Equal("FL123", resultList[0].FlightNumber);
        Assert.Equal("FL789", resultList[1].FlightNumber);
        Assert.Contains("Expected departure: LAX after ", resultList[0].InconsistentReason);
        Assert.Contains("Expected departure: DXB after ", resultList[1].InconsistentReason);
    }


}
