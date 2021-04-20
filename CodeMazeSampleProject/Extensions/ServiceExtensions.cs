using System.Linq;
using CodeMazeSampleProject.ActionFilters;
using CodeMazeSampleProject.Utilities;
using Contracts;
using Entities.Context;
using Entities.DataTransferObjects;
using LoggerService;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Repository;
using Repository.DataShaping;

namespace CodeMazeSampleProject.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod());
            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddScoped<ILoggerManager, LoggerManager>();

        public static void ConfigureSqlContext(this IServiceCollection services,
            IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("MssqlConnection"), b=>
                    b.MigrationsAssembly("Entities")));

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static IMvcBuilder AddCustomCsvFormatter(this IMvcBuilder builder) =>
            builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));

        public static void AddCustomServiceFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidateNewsForCategoryExistAttribute>();
            services.AddScoped<ValidateCategoryExistsAttribute>();
            services.AddScoped<ValidationFilterAttribute>();
            services.AddScoped<ValidateMediaTypeAttribute>();
        }

        public static void AddDataShaping(this IServiceCollection services)
        {
            services.AddScoped<IDataShaper<NewsDto>, DataShaper<NewsDto>>();
        }

        public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
                if (newtonsoftJsonOutputFormatter is not null)
                {
                    newtonsoftJsonOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.codemaze.hateoas+json");
                    newtonsoftJsonOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.codemaze.apiroot+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                    .OfType<XmlSerializerOutputFormatter>()?.FirstOrDefault();
                if (xmlOutputFormatter is not null)
                {
                    xmlOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.codemaze.hateoas+xml");
                    xmlOutputFormatter
                        .SupportedMediaTypes
                        .Add("application/vnd.codemaze.apiroot+xml");
                }
            });
        }

        public static void AddHateoas(this IServiceCollection services)
        {
            services.AddScoped<NewsLinks>();
        }

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
                // For header versioning : opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) =>
            services.AddResponseCaching();

        public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
        {
            services.AddHttpCacheHeaders((expirationOpt) =>
                {
                    expirationOpt.MaxAge = 65;
                    expirationOpt.CacheLocation = CacheLocation.Private;
                },
                (validationOpt) =>
                {
                    validationOpt.MustRevalidate = true;
                });
        }
    }
}