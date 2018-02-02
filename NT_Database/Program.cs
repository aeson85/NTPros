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
            program.InitialMapper();
            program.InitialConfiguration();
            var serviceCollection = program.InitialServiceProvider();
            program.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var rpcServer = serviceProvider.GetRequiredService<RPCServer>())
            {
                rpcServer.Start(serviceProvider);

                Console.WriteLine("RPC服务正在监听请求...按任意键结束");
                Console.ReadLine();
            }
        }

        private void InitialMapper()
        {
            Mapper.Initialize(config =>
            {
                config.AddProfile(new DbEntityProfile(_configuration));
            });
        }

        private void InitialConfiguration()
        {
            _configuration = ConfigurationSettings.Initial(Directory.GetCurrentDirectory()).Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddSingleton<RPCServer>();
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<DbOperator>();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddDbContext<AppDbContext>(opts =>
            {
                opts.UseMySQL(_configuration["Database:ConnectionString"]);
                opts.EnableSensitiveDataLogging(true);
            }, ServiceLifetime.Transient);
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DbEntityProfile(_configuration));
            }).CreateMapper());

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
