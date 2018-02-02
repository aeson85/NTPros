using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NT_Model.Entity;
using NT_Model.ViewModel;
using NT_MQPublisher.Infrastructure;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NT_MQPublisher.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly ConnectionConfig _connectionConfig;
        private readonly IModel _channel;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly string _replyQueueName;
        private readonly EventingBasicConsumer _consumer;
        private readonly IBasicProperties _props;
        private readonly BlockingCollection<string> _respQueue;


        public ProductsController(ConnectionConfig connectionConfig, IMapper mapper, ILoggerFactory loggerFactory)
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

        private DbOperationResultViewModel GetResult(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "", routingKey: "db_op_queue", basicProperties: _props, body: body);

            _channel.BasicConsume(queue: _replyQueueName, autoAck: true, consumer: _consumer);

            var res = _respQueue.Take();
            var result = JsonConvert.DeserializeObject<DbOperationResultViewModel>(res);

            return result;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _mapper.Map<Product>(model);
                var dbOpModel = new DbOperationViewModel();
                dbOpModel.Data = JsonConvert.SerializeObject(entity);
                dbOpModel.OperationRoute = "product.update";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("update")]
        public IActionResult Update([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                var product = _mapper.Map<Product>(model);
                dbOpModel.Data = JsonConvert.SerializeObject(product);
                dbOpModel.OperationRoute = "product.update";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
            /*
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                dbOpModel.Data = JsonConvert.SerializeObject(model);
                dbOpModel.OperationRoute = "product.update";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
            */
        }

        [HttpGet]
        public IActionResult Get([FromQuery]ProductSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                var product = _mapper.Map<Product>(model);
                dbOpModel.Data = JsonConvert.SerializeObject(product);
                dbOpModel.OperationRoute = "product.select";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                var resultData = JsonConvert.DeserializeObject<List<Product>>(result.Data);
                var response = _mapper.Map<List<ProductCreateViewModel>>(resultData);
                if (result.Success)
                {
                    return Json(response);
                }
                else
                {
                    _logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
            /*
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                dbOpModel.Data = JsonConvert.SerializeObject(model);
                dbOpModel.OperationRoute = "product.update";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    _logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
            */
        }
    }
}