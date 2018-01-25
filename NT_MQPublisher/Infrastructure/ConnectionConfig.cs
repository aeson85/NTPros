using System;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace NT_MQPublisher.Infrastructure
{
    public class ConnectionConfig : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConnection _connection;  
        private readonly IModel _channel;

        public ConnectionConfig(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionFactory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"],
                Port = int.Parse(_configuration["RabbitMQ:Port"])
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public IModel Channel => _channel;

        public string ExchangeName => _configuration["RabbitMQ:ExchangeName"];

        public void Dispose()
        {
            _connection?.Dispose();
            _channel?.Dispose();
        }
    }
}