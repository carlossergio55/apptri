using Aplicacion.Interfaces;
using Identity.Context;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Persistencia.Contexts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Webapi;

namespace Test
{
    [SetUpFixture]
    public class Testing
    {
        
        private static IConfiguration _configuration;
        private static IServiceScopeFactory _scopeFactory;
        private const string _currentUser = "TestingUser";
        private const int _currentSucrusal = 11;

        [OneTimeSetUp]
        public void RunBeforeAnyTest()
        {

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();

            var services = new ServiceCollection();
            var startup = new Startup(_configuration);

            services.AddSingleton(Mock.Of<IWebHostEnvironment>(w =>
            w.ApplicationName == "WebApi" &&
            w.EnvironmentName == "Development"));

            startup.ConfigureServices(services);

            var currentServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(ICurrentUserService));
            services.Remove(currentServiceDescriptor);
            services.AddTransient(d => Mock.Of<ICurrentUserService>(s => s.LoginUsuario == _currentUser && s.IdgenInstitucionsucursal == _currentSucrusal));
            

            _scopeFactory = services.BuildServiceProvider().GetService<IServiceScopeFactory>();

        }

        public static async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<AplicationDbContext>();
            context.Add(entity);
            await context.SaveChangesAsync();
        }

        public static async Task<TRespnose> SendAsync<TRespnose>(IRequest<TRespnose> request)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            return await mediator.Send(request);
        }

        public static async Task<TEntity> FindAsync<TEntity>(int id) where TEntity : class
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetService<AplicationDbContext>();
            return await context.FindAsync<TEntity>(id);
        }

        public static string GetCurrenUser() {
            return _currentUser;
        }
    
    }
}
