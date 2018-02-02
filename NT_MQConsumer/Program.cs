using System;
using System.Collections.Generic;
using System.Text;
using NT_MQConsumer.Infrastructure.Handler;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NT_Common;
using NT_WeChatUtilities;
using NT_CommonConfig.Infrastructure;
using System.Diagnostics;
using AutoMapper;
using NT_MQConsumer.Infrastructure;

namespace NT_WeChatMQConsumer
{
    class Program
    {
        private IConfiguration _configuration;

        private List<IConsumerHandler> _handlers;

        public IConfiguration Configuration => _configuration;

        static void Main(string[] args)
        {
            Console.Title = "Message Queue Client,PID: " +　Process.GetCurrentProcess().Id;

            var program = new Program();
            program.InitialConfiguration();
            var serviceCollection = program.InitialServiceProvider();
            program.ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            program._handlers = new List<IConsumerHandler>
            {
                serviceProvider.GetRequiredService<WeChatConsumerHandler>()
            };

            using (var client = serviceProvider.GetRequiredService<MsgClient>())
            {
                client.Start(program._handlers);

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
            services.AddSingleton<MsgClient>();
            services.AddSingleton<WeChatApiUrls>();
            services.AddSingleton<MQPublishServerUrls>();
            services.AddSingleton<WeChatUtilities>();
            services.AddTransient<WeChatConsumerHandler>();
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new WeChatProfile(_configuration));
            }).CreateMapper());
        }

        private IServiceCollection InitialServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            return serviceCollection;
        }
    }
}
