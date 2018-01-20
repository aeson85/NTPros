using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace NT_WebApp.Models
{
    public class Product_Price
    {
        public string ProductId { get; set; }
        
        public Product Product { get; set; }

        public string PriceId { get; set; }

        public NTPrice Price { get; set; }
    }
}