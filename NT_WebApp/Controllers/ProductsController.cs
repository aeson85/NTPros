using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using NT_Common.Extensions;
using System.Linq.Expressions;
using System.Net.Http;
using Newtonsoft.Json;
using NT_Model.Entity;
using NT_Model.ViewModel;
using NT_CommonConfig.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using IdentityModel;
using IdentityModel.Client;

namespace NT_WebApp.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : Controller
    {
        private readonly MQPublishServerUrls _mqPublishServerUrls;

        public ProductsController(MQPublishServerUrls mqPublishServerUrls)
        {
            _mqPublishServerUrls = mqPublishServerUrls;
        }
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsJsonAsync(_mqPublishServerUrls.GetProductCreateUrl(), model);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return new StatusCodeResult((int)HttpStatusCode.Created);
                    }
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]ProductCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.PutAsJsonAsync(_mqPublishServerUrls.GetProductUpdateUrl(), model);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return Ok();
                    }
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            return BadRequest();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ProductSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var accessToken = await this.HttpContext.GetTokenAsync("access_token");
                    client.SetBearerToken(accessToken);
                    var response = await client.GetAsync(string.Format(_mqPublishServerUrls.GetProductSelectUrl(), model.Name, model.Id));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        return Json(JsonConvert.DeserializeObject(responseData));
                    }
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync(string.Format(_mqPublishServerUrls.GetProductDeleteUrl(), id));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        return Ok();
                    }
                    return new StatusCodeResult((int)response.StatusCode);
                }
            }
            return BadRequest();
            /*
            if (ModelState.IsValid)
            {
                var product = _context.Product.Include(p => p.Product_Image_Lst).ThenInclude(p => p.Image).Include(p => p.Product_Price).ThenInclude(p => p.Price).SingleOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (product != null)
                {
                    //remove ntimage
                    var imageIds = product.Product_Image_Lst?.Select(p => p.ImageId).ToList();
                    if (imageIds.Count() > 0)
                    {
                        _context.NTImage.Where(p => imageIds.Contains(p.Id)).ToList().ForEach(p => 
                        {
                            _context.Entry(p).State = EntityState.Deleted;
                        });
                    }

                    //remoge ntprice
                    var price = product.Product_Price.Price;
                    if (price != null)
                    {
                        _context.Entry(price).State = EntityState.Deleted;
                    }
                    
                    //remove product
                    _context.Entry(product).State = EntityState.Deleted;
                    await _context.SaveChangesAsync();
                    new NoContentResult();
                }
            }
            return BadRequest();
            */
        }
    }
}