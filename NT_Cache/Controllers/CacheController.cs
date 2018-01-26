using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NT_Cache.Models;

namespace NT_Cache.Controllers
{
    [Route("api/[controller]")]
    public class CacheController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        
        public CacheController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Get([Required]string key)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cacheVal = await _distributedCache.GetStringAsync(key);
                    return Json(new CacheSuccessResponseModel 
                    {
                        Value = cacheVal
                    });
                }
                else
                {
                    return Json(new CacheFailResponseModel
                    {
                        ErrorMsg = ModelState
                    });
                }
            }
            catch (System.Exception ex)
            {
                return Json(new CacheFailResponseModel
                {
                    ErrorMsg = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CachedObjectModel model)
        {
            if (ModelState.IsValid)
            {
                await _distributedCache.SetStringAsync(model.Key, model.Value, model.Options ?? new DistributedCacheEntryOptions());
                return Ok();
            }
            return BadRequest(ModelState);
            
        }
    }
}