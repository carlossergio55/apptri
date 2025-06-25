using System;
using System.Reflection;
using Aplicacion;
using Aplicacion.Interfaces.Repositories;
using Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Persistencia;
using Shared;
using Webapi.Extensions;
using WebApi.Filters;




namespace Webapi
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddPersistenciaInfraestructura(Configuration);
            services.AddControllers(options =>
            {
                options.Filters.Add<WebApi.Filters.FiltroGlobalDeErrores>();
            });
            services.AddMediatRHandlers(Assembly.GetExecutingAssembly(), Assembly.GetCallingAssembly()); //agregado para utilizar consultas linq en WebApi/Services
            


            services.AddCors(p =>
            {
                p.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader();
                    });
            });
            services.AddHttpContextAccessor();
            services.AddCurrentUserService();

            services.AddDistributedMemoryCache();
            services.AddAplicacionLayer();
            services.AddIdentityInfraestructura(Configuration);
            services.AddPersistenciaInfraestructura(Configuration);
            services.AddSharedInfraestructura(Configuration);
            services.AddApiVersioningExtension();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CapacitacionApi", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "JWT Token Bearer {token}",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                        {
                            new OpenApiSecurityScheme{
                                    Reference = new OpenApiReference {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                            }
                        },
                        new string[]{ }
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
            }

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CapacitacionApi v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseErrorHandlingMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
