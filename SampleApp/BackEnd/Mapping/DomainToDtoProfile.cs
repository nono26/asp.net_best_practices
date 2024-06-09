using AutoMapper;
using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Models;

namespace SampleApp.BackEnd.Mapping;

public class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<TokenResource, TokenResourcesDto>();
    }
}