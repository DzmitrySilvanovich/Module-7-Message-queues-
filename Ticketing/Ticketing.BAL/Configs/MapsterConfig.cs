using Mapster;
using System.Reflection;
using System;
using Ticketing.BAL.Model;
using Ticketing.DAL.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace Ticketing.BAL.Configs
{
    public static class MapsterConfig
    {
        public static void RegisterMapsterConfiguration(this IServiceCollection services)
        {
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
        }
    }
}
