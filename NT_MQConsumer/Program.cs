using System;
using System.Collections.Generic;
using System.Text;
using NT_MQConsumer.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace NT_WeChatMQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = GetConfiguration();

            var consumerConfigurations = new List<IConsumerHandler>
            {
                new WeChatConsumerHandler(config),
            };

            var factory = new ConnectionFactory
            {
                HostName = config["RabbitMQ:HostName"],
                Port = int.Parse(config["RabbitMQ:Port"])
            };

            var exchangeName = config["RabbitMQ:ExchangeName"];
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
                        Console.WriteLine($"New Message : {message}" );
                        handler.Execute(message);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        Console.WriteLine($"Send Ack");
                    };
                    channel.BasicConsume(queue: handler.QueueName, autoAck: false, consumer: consumer);
                }
                Console.WriteLine("Waiting for message, press [enter] to exit.");
                Console.ReadLine();
            }
        }

        private static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder().AddJsonFile(Path.GetFullPath(Path.Combine(@"../NT_Common/globalSettings.json")), optional: true, reloadOnChange: true).SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        }
    }
}
