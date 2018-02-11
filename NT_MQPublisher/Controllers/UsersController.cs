using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NT_MQPublisher.Infrastructure;
using RabbitMQ.Client;
using NT_Common.Extensions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NT_Model.ViewModel;
using NT_Model.Entity;
using System.Linq;
using System.Collections.Generic;

namespace NT_MQPublisher.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : DBOpController
    {
        public UsersController(ConnectionConfig connectionConfig, IMapper mapper, ILoggerFactory loggerFactory) : base(connectionConfig, mapper, loggerFactory)
        {
        }

        [HttpPost]
        public IActionResult Post([FromBody]AppUserViewModel model)
        {
            return Ok();
        }

        [HttpPost("WeChatSave")]
        public IActionResult WeChatSave([FromBody]AppUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = this.Mapper.Map<AppUser>(model);
                entity.Id = model.Id;
                var dbOpModel = new DbOperationViewModel();
                dbOpModel.Data = JsonConvert.SerializeObject(entity);
                dbOpModel.OperationRoute = "appuser.upsertforwechat";
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
    }
}