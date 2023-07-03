using AutoMapper;
using IntegrationProject.Dtos;
using IntegrationProject.Models;


namespace IntegrationProject
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Flight, FlightDto>();
            CreateMap<FlightDto, Flight>();
            CreateMap<Airline, AirlineDto>();
            CreateMap<AirlineDto, Airline>();
        }
    }
}