using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace NT_WebApp.Models
{
    public class Product_Image
    {
        public string ProductId { get; set; }
        
        public Product Product { get; set; }

        public string ImageId { get; set; }

        public NTImage Image { get; set; }

        public ProductImgType Type { get; set; } 
    }
}