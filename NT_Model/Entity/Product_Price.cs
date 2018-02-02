using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NT_Common.Extensions;

namespace NT_Model.Entity
{
    public class Product_Price : IBaseEntity
    {
        //ProductId和PriceId 为联合主键，此Id仅为了满足IBaseEntity接口
        [NotMapped]
        [JsonIgnore]
        public string Id { get; set; }

        private string _mockId;
        [NotMapped]
        public string MockId {
            get
            {
                _mockId = _mockId ?? $"{this.ProductId}{this.PriceId}".ToCrc32().ToString();
                return _mockId;
            }
            set => _mockId = value;
        }
        
        public string ProductId { get; set; }
        
        public Product Product { get; set; }

        public string PriceId { get; set; }

        public NTPrice Price { get; set; }
    }
}