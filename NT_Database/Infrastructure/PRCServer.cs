using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NT_Database.Infrastructure
{
    public class RPCServer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        
        public RPCServer(IConfiguration configuration)
        {
            _configuration = configuration;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"])
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }    

        public void Start()
        {
            _channel.QueueDeclare(queue: "rpc_queue", durable: false, autoDelete: false, arguments: null);
            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) => 
            {
                string response = null;
                var body = ea.Body;
                var props = ea.BasicProperties;
                var replyProperties = _channel.CreateBasicProperties();
                replyProperties.CorrelationId = props.CorrelationId;
                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine("新消息: " + message);
                    response = string.Empty;
                }
                catch(Exception e)
                {
                    Console.WriteLine("异常信息: " + e.Message);
                    response = "";
                }
                finally
                {
                    var resposneBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProperties, body: resposneBytes);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            _channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}