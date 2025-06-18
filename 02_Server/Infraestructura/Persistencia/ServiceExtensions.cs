using Aplicacion.Interfaces;
using Aplicacion.Interfaces.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistencia.Contexts;
using Persistencia.Repository.Common.Aplicacion;
using Persistencia.Repository.Common.Seguridad;
using Persistencia.Repository.Custom;



namespace Persistencia
{
    public static class ServiceExtensions
    {
        public static void AddPersistenciaInfraestructura(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<AplicationDbContext>(options =>
                                                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                                                        .UseSnakeCaseNamingConvention());
            services.AddTransient(typeof(IRepositoryAsync<>), typeof(AppRepositoryAsync<>));
            services.AddTransient<ISegurityRepository, SegurityRepository>();
            services.AddTransient<ISeguritySystemRepository, SeguritySystemRepository>();
            services.AddTransient<ISegurityMenuRepository, SegurityMenuRepository>();
            services.AddTransient<IUnitOfWork, AppUnitOfWork>();


            //TODO: Agregar aqui Repositorios especificos elaborados Ej.: services.AddTransient<ILibroRepository, LibroRepository>();
            //services.AddTransient<IProgramacionAnualRepository, ProgramacionAnualRepository>();
            //services.AddTransient<IProgSeguimientoRepository, ProgSeguimientoRepository>();
            //services.AddTransient<IPacFormularioRepository, PacFormularioRepository>();
            services.AddTransient<IClienteRepository, ClienteRepository>();
        }


    }
}
