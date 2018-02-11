using System;
using System.IO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NT_CommonConfig.Infrastructure;
using NT_Database.Infrastructure;
using NT_Database.Infrastructure.Handler;
using NT_Database.Infrastructure.Repository;
using Microsoft.Extensions.Logging.Console;
using Newtonsoft.Json;
using System.Diagnostics;

namespace NT_Database
{
    class Program
    {
        private IConfiguration _configuration;

        static void Main(string[] args)
        {
            Console.Title = "Database Opeartion Server,PID: " +　Process.GetCurrentProcess().Id;
            var program = new Program();
            program.InitialConfiguration();
            var serviceCollection = program.InitialServiceProvider();
            program.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
    
            using (var rpcServer = serviceProvider.GetRequiredService<RPCServer>())
            {
                rpcServer.Start();

                Console.WriteLine("Waiting for message, press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private void InitialConfiguration()
        {
            _configuration = ConfigurationSettings.Initial(Directory.GetCurrentDirectory()).Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddSingleton<RPCServer>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DbEntityProfile(_configuration));
            }).CreateMapper());

            services.AddScoped<DbOperator>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ProductDbHandler>();
            services.AddScoped<AppUserDbHandler>();
            services.AddScoped<ProductRepository>();
            services.AddScoped<AppUserRepository>();

            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseMySQL(_configuration["Database:ConnectionString"]);
                opts.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Scoped);

            services.AddDbContext<AppUserDbContext>(opts =>
            {
                opts.UseMySQL(_configuration["Database:UserConnectionString"]);
                opts.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Scoped);

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        private IServiceCollection InitialServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            return serviceCollection;
        }
    }
}
