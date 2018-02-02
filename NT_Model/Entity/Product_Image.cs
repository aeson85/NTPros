using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using NT_Common.Extensions;

namespace NT_Model.Entity
{
    public class Product_Image : IBaseEntity
    {
        public Product_Image()
        {
            _guid = Guid.NewGuid().ToString();
        }

        //ProductId和ImageId 为联合主键，此Id仅为了满足IBaseEntity接口
        [NotMapped]
        [JsonIgnore]
        public string Id { get; set; }

        private string _mockId;
        [NotMapped]
        public string MockId {
            get
            {
                _mockId = _mockId ?? $"{this.ProductId}{this.ImageId}".ToCrc32().ToString();
                return _mockId;
            }
            set => _mockId = value;
        }

        public string ProductId { get; set; }
        
        public Product Product { get; set; }

        public string ImageId { get; set; }

        public NTImage Image { get; set; }

        public int Type { get; set; }

        private string _guid;
        [NotMapped]
        [JsonIgnore]
        public string GuidD => _guid;
    }
}