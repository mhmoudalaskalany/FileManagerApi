using System.Reflection;
using AutoMapper;
using FileManager.Common.Abstraction.UnitOfWork;
using FileManager.Common.Extensions;
using FileManager.Data.Context;
using FileManager.Data.DataInitializer;
using FileManager.Data.UnitOfWork;
using FileManager.Service.Mapping;
using FileManager.Service.Services.Base;
using FileManager.Service.Services.File;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetCore.AutoRegisterDi;

namespace FileManager.Api.Extensions
{
    /// <summary>
    /// Dependency Extensions
    /// </summary>
    public static class ConfigureServiceExtension
    {
        /// <summary>
        /// Register Extensions
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDatabaseConfig(configuration);
            //services.RegisterRepository();
            services.RegisterCores();
            services.RegisterAutoMapper();
            services.RegisterCommonServices(configuration);
            services.ConfigureAuthentication(configuration);
            services.AddControllers();
            return services;
        }

        /// <summary>
        /// Database Config
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void RegisterDatabaseConfig(this IServiceCollection services, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("FileManagerDbContext");
            services.AddDbContext<FileManagerDbContext>(options => options.UseSqlServer(connection));
            services.AddScoped<DbContext, FileManagerDbContext>();
            services.AddSingleton<IDataInitializer, DataInitializer>();
        }

        /// <summary>
        /// Configure Authentication With Identity Server
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
                .AddJwtBearer("Bearer", config =>
                {
                    config.Authority = configuration["Authority"];// the url of identity server
                    config.Audience = "FileManagerApi"; // name of the api that is defined in the identity server api resource
                    config.RequireHttpsMetadata = false;

                });
        }

        /// <summary>
        /// register auto-mapper
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingService));
        }

        ///// <summary>
        ///// Register Custom Repositories
        ///// </summary>
        ///// <param name="services"></param>

        //private static void RegisterRepository(this IServiceCollection services)
        //{

        //}

        /// <summary>
        /// Register Main Core
        /// </summary>
        /// <param name="services"></param>
        private static void RegisterCores(this IServiceCollection services)
        {
            services.AddTransient(typeof(IBaseService<,,,,>), typeof(BaseService<,,,,>));
            services.AddTransient(typeof(IServiceBaseParameter<,>), typeof(ServiceBaseParameter<,>));
            services.AddTransient(typeof(IUnitOfWork<,>), typeof(UnitOfWork<,>));
            var servicesToScan = Assembly.GetAssembly(typeof(FileService)); //..or whatever assembly you need
            services.RegisterAssemblyPublicNonGenericClasses(servicesToScan)
                .Where(c => c.Name.EndsWith("Service"))
                .AsPublicImplementedInterfaces();
        }

    }
}
