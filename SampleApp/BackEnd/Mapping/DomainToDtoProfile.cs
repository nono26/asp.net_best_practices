using AutoMapper;
using SampleApp.BackEnd.Domain;
using SampleApp.BackEnd.Models;

namespace SampleApp.BackEnd.Mapping;

public class DomainToDtoProfile : Profile
{
    public DomainToDtoProfile()
    {
        CreateMap<TokenResource, TokenResourcesDto>()
        .ForMember(a => a.AccessToken, opt => opt.MapFrom(a => a.Token))
        .ForMember(a => a.RefreshToken, opt => opt.MapFrom(a => a.RefreshToken.Token))
        .ForMember(a => a.Expiration, opt => opt.MapFrom(a => a.Expiration));
    }
}