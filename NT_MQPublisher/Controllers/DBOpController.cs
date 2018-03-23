using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NT_MQPublisher.Infrastructure;
using RabbitMQ.Client;
using NT_Common.Extensions;
using System.Threading.Tasks;
using AutoMapper;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System;
using NT_Model.ViewModel;

namespace NT_MQPublisher.Controllers
{
    public abstract class DBOpController : Controller
    {
        private readonly ConnectionConfig _connectionConfig;
        private readonly IModel _channel;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue;

        protected IMapper Mapper => _mapper;
        protected ILogger Logger => _logger;

        public DBOpController(ConnectionConfig connectionConfig, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _mapper = mapper;
            _logger = loggerFactory.CreateLogger(this.GetType());
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

        protected DbOperationResultViewModel GetResult(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "db_op_queue", basicProperties: _props, body: body);

            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);

            var res = _respQueue.Take();
            var result = JsonConvert.DeserializeObject<DbOperationResultViewModel>(res);

            return result;
        }
    }
}