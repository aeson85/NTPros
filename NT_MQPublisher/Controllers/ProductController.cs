using System;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NT_Model.ViewModel;
using NT_MQPublisher.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NT_MQPublisher.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ConnectionConfig _connectionConfig;
        private readonly IModel _channel;

        #region rpc config

        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue;

        #endregion

        public ProductController(ConnectionConfig connectionConfig)
        {
            _connectionConfig = connectionConfig;
            _channel = _connectionConfig.Channel;

            _respQueue = new BlockingCollection<string>();
            _replyQueueName = _channel.QueueDeclare().QueueName;
            _consumer = new EventingBasicConsumer(_channel);

            _props = _channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            _props.CorrelationId = correlationId;
            _props.ReplyTo = _replyQueueName;

            _consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    _respQueue.Add(response);
                }
            };
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var modelStr = JsonConvert.SerializeObject(model);
                var body = Encoding.UTF8.GetBytes(modelStr);

                _channel.BasicPublish(exchange: "", routingKey: "rpc_queue", basicProperties: _props, body: body);

                _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);

                var res = _respQueue.Take();
            }
            return BadRequest();
        }
    }
}