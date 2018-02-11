using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    public class ProductsController : DBOpController
    {
        public ProductsController(ConnectionConfig connectionConfig, IMapper mapper, ILoggerFactory loggerFactory) : base(connectionConfig, mapper, loggerFactory)
        {
        }

        [HttpPost]
        public IActionResult Post([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = this.Mapper.Map<Product>(model);
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
                    this.Logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut]
        public IActionResult Put([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                var product = this.Mapper.Map<Product>(model);
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
                    this.Logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
        }

        [HttpGet]
        public IActionResult Get([FromQuery]ProductSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                var product = this.Mapper.Map<Product>(model);
                dbOpModel.Data = JsonConvert.SerializeObject(product);
                dbOpModel.OperationRoute = "product.select";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                var resultData = JsonConvert.DeserializeObject<List<Product>>(result.Data);
                var response = this.Mapper.Map<List<ProductCreateViewModel>>(resultData);
                if (result.Success)
                {
                    return Json(response);
                }
                else
                {
                    this.Logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([Required]string id)
        {
            if (ModelState.IsValid)
            {
                var dbOpModel = new DbOperationViewModel();
                dbOpModel.Data = id;
                dbOpModel.OperationRoute = "product.delete";
                
                var message = JsonConvert.SerializeObject(dbOpModel);
                
                var result = this.GetResult(message);
                if (result.Success)
                {
                    return Ok();
                }
                else
                {
                    this.Logger.LogError(result.ErrorMsg);
                }
                return new StatusCodeResult(500);
            }
            return BadRequest(ModelState);
        }
    }
}