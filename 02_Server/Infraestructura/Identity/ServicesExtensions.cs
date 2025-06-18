using Aplicacion.Interfaces;
using Aplicacion.Wrappers;
using Dominio.Settings;
using Identity.Context;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity
{
    public static class ServicesExtensions
    {
        public static void AddIdentityInfraestructura(this IServiceCollection servicies, IConfiguration configuration)
        {

            servicies.AddDbContext<IdentityContext>(options =>
                           options.UseNpgsql(
                               configuration.GetConnectionString("IdentityConnection"),
                               b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));

            servicies.AddDbContext<SegurityContext>(options =>
                            options.UseNpgsql(
                                configuration.GetConnectionString("SegurityConnection")
                                ).UseSnakeCaseNamingConvention());

            servicies.AddIdentity<AplicationUser, IdentityRole>()
                            .AddEntityFrameworkStores<IdentityContext>()
                            .AddDefaultTokenProviders();

            servicies.AddTransient<IAcountService, AcountService>();

            servicies.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
            
            servicies.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };

                o.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c => {                        
                        c.NoResult();
                        c.Response.StatusCode = 500;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    },
                    OnChallenge = context => {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new Response<string>("No esta autorizado"));
                        return context.Response.WriteAsync(result);
                    },
                    OnForbidden  = ctx => {
                        ctx.Response.StatusCode = 400;
                        ctx.Response.ContentType = "application/json";
                        var result = JsonConvert.SerializeObject(new Response<string>("No tiene permisos a los recursos"));
                        return ctx.Response.WriteAsync(result);                    
                    },
                    OnTokenValidated = ctx => {

                        var accessToken = ctx.SecurityToken as JwtSecurityToken;
                        if (accessToken != null)
                        {
                            ClaimsIdentity identity = ctx.Principal.Identity as ClaimsIdentity;
                            if (identity != null)
                            {
                                identity.AddClaim(new Claim("access_token", accessToken.RawData));
                            }
                        }


                        return Task.CompletedTask;
                    }

                };


            });

        }

    }
}
