using BackEnd.Refactoring.Application.Abstration.Srlztn;
using Microsoft.Extensions.DependencyInjection;

namespace BackEnd.Refactoring.Infra.Serialization.Legacy;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddLegacySerializer(this IServiceCollection service)
        => service.AddSingleton<ISerializer, LegacyNewtonSoftSerializer>();
}
