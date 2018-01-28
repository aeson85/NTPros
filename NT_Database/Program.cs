using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NT_Common.Infrastructure;
using NT_Database.Infrastructure;

namespace NT_Database
{
    class Program
    {
        private IConfiguration _configuration;

        static void Main(string[] args)
        {
            Console.Title = "数据库操作服务";
            var program = new Program();
            program.InitialConfiguration();
            var serviceCollection = program.InitialServiceProvider();
            program.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            using (var rpcServer = serviceProvider.GetRequiredService<RPCServer>())
            {
                rpcServer.Start();

                Console.WriteLine("RPC服务正在监听请求...按任意键结束");
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
        }

        private IServiceCollection InitialServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            return serviceCollection;
        }
    }
}
