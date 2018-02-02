using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NT_MQConsumer.Infrastructure.Handler;
using NT_WeChatUtilities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NT_MQConsumer.Infrastructure
{
    public class MsgClient : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly string _exchangeName;

        public MsgClient(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"])
            };
            
            _exchangeName = _configuration["RabbitMQ:ExchangeName"];
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }    
        
        public void Start(List<IConsumerHandler> handlers)
        {
            _channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);
            foreach (var handler in handlers)
            {
                _channel.QueueDeclare(queue: handler.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                _channel.QueueBind(queue: handler.QueueName, exchange: _exchangeName, routingKey: handler.RoutingKey);
                var consumer = new EventingBasicConsumer(_channel);consumer.Received += (moel, ea) =>
                {
                    var message = Encoding.UTF8.GetString(ea.Body);
                    handler.Execute(message);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                _channel.BasicConsume(queue: handler.QueueName, autoAck: false, consumer: consumer);
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}