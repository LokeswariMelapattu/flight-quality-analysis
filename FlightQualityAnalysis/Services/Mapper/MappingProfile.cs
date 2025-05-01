using AutoMapper;
using FlightQualityAnalysis.Domain.Entities;
using FlightQualityAnalysis.Services.DTOs;

namespace FlightQualityAnalysis.Services.Mapper;
// <summary>
// Mapping profile for AutoMapper to map between FlightDetails and DTOs
// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Create mappings between FlightDetails and FlightDetailsResultDTO
        CreateMap<FlightDetails, FlightDetailsResultDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AircraftRegistrationNumber, opt => opt.MapFrom(src => src.Aircraft_Registration_Number))
            .ForMember(dest => dest.AircraftType, opt => opt.MapFrom(src => src.Aircraft_Type))
            .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.Flight_Number))
            .ForMember(dest => dest.DepartureAirport, opt => opt.MapFrom(src => src.Departure_Airport))
            .ForMember(dest => dest.DepartureDateTime, opt => opt.MapFrom(src => src.Departure_DateTime))
            .ForMember(dest => dest.ArrivalAirport, opt => opt.MapFrom(src => src.Arrival_Airport))
            .ForMember(dest => dest.ArrivalDateTime, opt => opt.MapFrom(src => src.Arrival_DateTime));

        // Create mappings between FlightDetails and AnalyzeFlightSequencesResultDTO
        CreateMap<FlightDetails, AnalyzeFlightSequencesResultDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.AircraftRegistrationNumber, opt => opt.MapFrom(src => src.Aircraft_Registration_Number))
            .ForMember(dest => dest.AircraftType, opt => opt.MapFrom(src => src.Aircraft_Type))
            .ForMember(dest => dest.FlightNumber, opt => opt.MapFrom(src => src.Flight_Number))
            .ForMember(dest => dest.DepartureAirport, opt => opt.MapFrom(src => src.Departure_Airport))
            .ForMember(dest => dest.DepartureDateTime, opt => opt.MapFrom(src => src.Departure_DateTime))
            .ForMember(dest => dest.ArrivalAirport, opt => opt.MapFrom(src => src.Arrival_Airport))
            .ForMember(dest => dest.ArrivalDateTime, opt => opt.MapFrom(src => src.Arrival_DateTime));
    }
}
