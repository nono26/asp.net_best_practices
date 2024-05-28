using BackEnd.Infrastructure.Logger;
using BackEnd.Logic.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace BackeEnd.Infrastructure;

public static class InfrastructureExtension
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ILogger<>), typeof(ConsoleLogger<>));
        return services;
    }
}
