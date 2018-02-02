using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using NT_Database.Infrastructure.Repository;
using NT_Model.Entity;
using NT_Model.ViewModel;
using NT_Common.Extensions;

namespace NT_Database.Infrastructure.Handler
{
    public class ProductDbHandler : DbHandler<Product>
    {
        private readonly IRepository<Product> _repository;
        private readonly IMapper _mapper;

        public ProductDbHandler(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _repository = unitOfWork.Repository<Product>();
            _mapper = mapper;
        }

        public override DbOperationResultViewModel Select(string entityStr)
        {
            var result = this.CreateReponse();
            var entity = JsonConvert.DeserializeObject<Product>(entityStr);
            Expression<Func<Product, bool>> predicate = p => true;
            if (!string.IsNullOrWhiteSpace(entity.Id))
            {
                predicate = predicate.AndAlso(p => p.Id.Equals(entity.Id, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrWhiteSpace(entity.Name))
            {
                predicate = predicate.AndAlso(p => EF.Functions.Like(p.Name.ToLower(), $"%{entity.Name.ToLower()}%"));
            }
            var products = _repository.Get(predicate);
            result.Data = JsonConvert.SerializeObject(products);
            return result;
        }

        public override DbOperationResultViewModel Update(string entityStr)
        {
            var result = this.CreateReponse();
            var model = JsonConvert.DeserializeObject<Product>(entityStr);
            var entity = _repository.SingleOrDefault(p => p.Id == model.Id, disableTracking: false);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                var productImageRepository = this.UnitOfWork.Repository<Product_Image>();
                var productPriceRepository = this.UnitOfWork.Repository<Product_Price>();
                var imageRepository = this.UnitOfWork.Repository<NTImage>();
                var priceRepository = this.UnitOfWork.Repository<NTPrice>();

                foreach (var product_image in productImageRepository.Local(EntityState.Deleted))
                {
                    imageRepository.Remove(product_image.ImageId);
                }
                
                foreach (var product_price in productPriceRepository.Local(EntityState.Deleted))
                {
                    imageRepository.Remove(product_price.PriceId);
                }
                this.UnitOfWork.Commit();
            }
            else
            {
                result.ErrorMsg = $"Id: {model.Id} not found";
            }
            return result;
            /*
            var result = this.CreateReponse();
            var model = JsonConvert.DeserializeObject<ProductCreateViewModel>(entityStr);
            var entity = _repository.SingleOrDefault(p => p.Id == model.Id, disableTracking: false);
            if (entity != null)
            {
                entity = _mapper.Map(model, entity);
                var productImageRepository = this.UnitOfWork.Repository<Product_Image>();
                var imageRepository = this.UnitOfWork.Repository<NTImage>();
                foreach (var item in productImageRepository.Local(EntityState.Deleted))
                {
                    imageRepository.Remove(item.ImageId);
                }
                this.UnitOfWork.Commit();
            }
            else
            {
                result.ErrorMsg = $"Id: {model.Id} not found";
            }
            return result;
            */
        }
    }
}