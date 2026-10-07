using System;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Refactoring.Infra.Serialization.Moderm;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddModermSerializer(this IServiceCollection service)
        => service.AddSingleton<ISerializer, SystemTextJsonSerializer>();
}
