using Aplicacion.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;

namespace Shared
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCurrentUserService(this IServiceCollection services)
        {            
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }

        public static void AddSharedInfraestructura(this IServiceCollection services, IConfiguration configuration)
        {                       
            services.AddTransient<IDateTimeService, DateTimeService>();
            services.AddTransient<ICastMappingUpdate, CastMappingUpdate>();
        }
    }
}
