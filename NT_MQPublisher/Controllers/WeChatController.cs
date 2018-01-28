using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NT_MQPublisher.Infrastructure;
using RabbitMQ.Client;
using NT_Common.Extensions;
using System.Threading.Tasks;

namespace NT_MQPublisher.Controllers
{
    [Route("api/[controller]")]
    public class WeChatController : Controller
    {
        private readonly ConnectionConfig _connectionConfig;
        private readonly IModel _channel;

        public WeChatController(ConnectionConfig connectionConfig)
        {
            _connectionConfig = connectionConfig;
            _channel = _connectionConfig.Channel;
        }

        [HttpPost("publish")]
        public async Task Publish(string routingKey, string queueName, string exchangeName)
        {
            try
            {
                _connectionConfig.Channel.ExchangeDeclare(exchange: _connectionConfig.ExchangeName, type: ExchangeType.Topic, durable: true, autoDelete: false, arguments: null);

                _channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
    
                var messageBody = await this.Request.Body.ToByteArray();

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;

                _channel.BasicPublish(exchange: exchangeName, routingKey: routingKey, basicProperties: properties, body: messageBody);
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }
    }
}