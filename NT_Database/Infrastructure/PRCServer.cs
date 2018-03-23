using System;
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NT_Database.Infrastructure.Handler;
using NT_Database.Infrastructure.Repository;
using NT_Model.ViewModel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NT_Database.Infrastructure
{
    public class RPCServer : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IConfiguration _configuration;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;

        public RPCServer(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _serviceProvider = serviceProvider;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                //Port = int.Parse(_configuration["RabbitMQ:Port"])
            };
            
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }    

        public void Start()
        {
            var queueName = "db_op_queue";
            _channel.QueueDeclare(queue: queueName, durable: false, autoDelete: false, arguments: null);
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
                    using (var scope = _serviceProvider.CreateScope())
                    using (var dbOperator = scope.ServiceProvider.GetRequiredService<DbOperator>())
                    {
                        response = dbOperator.Execute(message);
                    }
                }
                catch (DbOperationException ex)
                {
                    response = JsonConvert.SerializeObject(ex.Result);
                }
                catch (Exception ex)
                {
                    response = JsonConvert.SerializeObject(new DbOperationResultViewModel 
                    {
                        ErrorMsg = ex.Message
                    });
                }
                finally
                {
                    var resposneBytes = Encoding.UTF8.GetBytes(response);
                    _channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProperties, body: resposneBytes);
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            };

            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}