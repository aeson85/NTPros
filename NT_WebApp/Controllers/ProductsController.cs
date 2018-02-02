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
using NT_WebApp.Infrastructure.MQ;
using Newtonsoft.Json;
using NT_Model.Entity;
using NT_Model.ViewModel;

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
            /* 
            if (ModelState.IsValid)
            {
                var product = _context.Product.Include(p => p.Product_Image_Lst).ThenInclude(p => p.Image).Include(p => p.Product_Price).ThenInclude(p => p.Price).SingleOrDefault(p => p.Id.Equals(model.Id, StringComparison.OrdinalIgnoreCase));
                if (product != null)
                {
                    product = _mapper.Map(model, product);
                    foreach (var item in _context.Product_Image.Local.ToList())
                    {
                        if (_context.Entry(item).State == EntityState.Deleted)
                        {
                            var ntImage = _context.NTImage.SingleOrDefault(p => item.ImageId == p.Id);
                            _context.Entry(ntImage).State = EntityState.Deleted;
                        }
                    }
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            return BadRequest();
            */
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ProductSearchViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
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
            /*
            Expression<Func<Product, bool>> predicate = p => true;
            if (!string.IsNullOrWhiteSpace(model.Id))
            {
                predicate = predicate.AndAlso(p => p.Id.Equals(model.Id, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(model.Name))
            {
                predicate = predicate.AndAlso(p => EF.Functions.Like(p.Name.ToLower(), $"%{model.Name.ToLower()}%"));
            }
            var products = _context.Product.AsNoTracking().Include(p => p.Product_Image_Lst).ThenInclude(p => p.Image).Include(p => p.Product_Price).ThenInclude(p => p.Price).Where(predicate).ToList();
            var productModels = _mapper.Map<List<ProductCreateViewModel>>(products);
            return Ok(productModels);
            */
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    var response = await client.DeleteAsync(_mqPublishServerUrls.GetProductDeleteUrl());
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