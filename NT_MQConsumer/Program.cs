using System;
using System.Collections.Generic;
using System.Text;
using NT_MQConsumer.Infrastructure;
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

namespace NT_WeChatMQConsumer
{
    class Program
    {
        private IConfiguration _configuration;

        public IConfiguration Configuration => _configuration;

        static void Main(string[] args)
        {
            Console.Title = "Message Queue Client,PID: " +　Process.GetCurrentProcess().Id;
            var program = new Program();
            program.InitialConfiguration();
            var serviceCollection = program.InitialServiceProvider();
            program.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var consumerConfigurations = new List<IConsumerHandler>
            {
                new WeChatConsumerHandler(program.Configuration, serviceProvider.GetRequiredService<WeChatUtilities>()),
            };

            var factory = new ConnectionFactory
            {
                HostName = program.Configuration["RabbitMQ:HostName"],
                Port = int.Parse(program.Configuration["RabbitMQ:Port"])
            };

            var exchangeName = program.Configuration["RabbitMQ:ExchangeName"];
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

                foreach (var handler in consumerConfigurations)
                {
                    channel.QueueDeclare(queue: handler.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    channel.QueueBind(queue: handler.QueueName, exchange: exchangeName, routingKey: handler.RoutingKey);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (moel, ea) =>
                    {
                        var message = Encoding.UTF8.GetString(ea.Body);
                        handler.Execute(message);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: handler.QueueName, autoAck: false, consumer: consumer);
                }
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
            services.AddSingleton<WeChatApiUrls>();
            services.AddSingleton<WeChatUtilities>();
        }

        private IServiceCollection InitialServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            return serviceCollection;
        }
    }
}
