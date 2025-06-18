using Blazored.LocalStorage;
using FluentValidation;
using Infraestructura.Component;
using Infraestructura.Models.Authentication;
using Infraestructura.Services;
using Infraestructura.Validator.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Infraestructura.Extensions
{
    public static class SiabysServicesExtension
    {
        public static IServiceCollection AddSiabysServices(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();

            services.AddAuthorizationCore();
            services.AddScoped<ManagerStateAuthorize>();
            services.AddScoped<AuthenticationStateProvider, ManagerStateAuthorize>(provider => provider.GetRequiredService<ManagerStateAuthorize>());
            services.AddScoped<IManagerAuthorize, ManagerStateAuthorize>(provider => provider.GetRequiredService<ManagerStateAuthorize>());
            services.AddScoped<IManagerStorage, ManagerStorage>();

            //AGREGAR COMPONENTES GLOBALES
            #region Componentes Globales
            services.AddScoped<Loading>();
            #endregion

            //TODO: Agregar aqui inyectores de validadores 
            #region Validadores
            services.AddTransient<IValidator<LoginRequest>, LoginValidator>();
            services.AddScoped<ManagerRest>();
            #endregion

            return services;
        }

    }
}
