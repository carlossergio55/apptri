using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Webapi.Extensions
{
    public static class MediatRServiceExtensions
    {
        public static IServiceCollection AddMediatRHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddMediatR(assemblies);
            return services;
        }
    }
}