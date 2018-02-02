using System;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Linq;
using System.Reflection;
using Force.Crc32;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using NT_Common.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace NT_Test
{
    class Program
    {
        private IConfiguration _configuration;
        
        static void Main(string[] args)
        {
            var program = new Program();
            program.InitialConfiguration();
            var serviceCollection = program.InitialServiceProvider();
            program.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var handler = serviceProvider.GetRequiredService<Handler>();
            handler.NewAuthor();
            handler.Commit();
            
            Console.ReadLine();
        }

        private void InitialConfiguration()
        {
            _configuration = ConfigurationSettings.Initial(Directory.GetCurrentDirectory()).Build();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IConfiguration>(_configuration);
            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseMySQL("server=localhost;database=unitofworkdb;user=root;password=Cosmicworks123");
            });
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<Handler>();
        }

        private IServiceCollection InitialServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            return serviceCollection;
        }
    }    
}
