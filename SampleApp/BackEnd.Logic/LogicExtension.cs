using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BackEnd.Logic;

public static class LogicExtension
{
    public static IServiceCollection AddLogic(this IServiceCollection services)
    {
        services.AddMediatR(cfg => { cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()); });
        return services;
    }
}