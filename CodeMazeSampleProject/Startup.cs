using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreRateLimit;
using CodeMazeSampleProject.ActionFilters;
using CodeMazeSampleProject.Extensions;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.HttpOverrides;
using NLog;

namespace CodeMazeSampleProject
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(),
                "/nlog.config"));
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.ConfigureRateLimitingOptions();
            services.AddHttpContextAccessor();
            services.ConfigureResponseCaching();
            services.ConfigureHttpCacheHeaders();
            services.ConfigureVersioning();
            services.AddHateoas();
            services.AddDataShaping();
            services.AddCustomServiceFilters();
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddAutoMapper(typeof(Startup));
            services.ConfigureRepositoryManager();
            services.ConfigureSqlContext(Configuration);
            services.ConfigureCors();
            services.ConfigureLoggerService();
            services.AddControllers(config =>
                {
                    config.RespectBrowserAcceptHeader = true;
                    config.ReturnHttpNotAcceptable = true;
                    config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
                    {
                        Duration = 120
                    });
                })
                .AddNewtonsoftJson()
                .AddXmlSerializerFormatters()
                .AddCustomCsvFormatter();
            services.AddCustomMediaTypes();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CodeMazeSampleProject", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerManager logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CodeMazeSampleProject v1"));
            }
            
            app.ConfigureExceptionHandler(logger);
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseResponseCaching();
            app.UseHttpCacheHeaders();
            app.UseIpRateLimiting();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
