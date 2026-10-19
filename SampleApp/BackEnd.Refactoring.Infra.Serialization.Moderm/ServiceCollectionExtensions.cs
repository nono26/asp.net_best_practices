using BackEnd.Refactoring.Application.Abstraction.Srlztn;
using Microsoft.Extensions.DependencyInjection;

namespace BackEnd.Refactoring.Infra.Serialization.Moderm;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModermSerializer(this IServiceCollection service)
        => service.AddSingleton<ISerializer, SystemTextJsonSerializer>();
}
