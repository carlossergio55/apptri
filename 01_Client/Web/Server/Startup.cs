using Infraestructura.Extensions;
using Infraestructura.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;
using Server.Models;
using Server.Shared;
using Server.Shared.Component;
using Syncfusion.Blazor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using ConfigurationPath = Server.Models.ConfigurationPath;


namespace Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("Api", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection("EndPoints:Api").Value);

            }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });

            services.AddHttpClient("ApiSeg", c =>
            {
                c.BaseAddress = new Uri(Configuration.GetSection("EndPoints:Apiseg").Value);
            }).ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
            });

            services.AddScoped<TBreadCrumCustom>();
            services.AddScoped<IConfigurationPath, ConfigurationPath>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSyncfusionBlazor();
            services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));
            services.AddScoped<Radzen.TooltipService>();
            services.AddScoped<GitHubService>();


            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>()
                {                    
                    new CultureInfo("es"),
                };
                options.DefaultRequestCulture = new RequestCulture("es");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddRazorPages();
            services.AddServerSideBlazor(c => c.DetailedErrors = true);
            services.AddSiabysServices();
            services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopEnd;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = true;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Text;
            });
           
           
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NTk1NjUwQDMxMzkyZTM0MmUzMGo1YTk3b29rL2t6NVkrUG1md2ZsUXFScHY4WG1lSmdCZTczSEQvdW9DQ2s9");
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
