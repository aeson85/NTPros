using System;
using System.Collections.Generic;

namespace NT_WebApp.Models.ViewModels
{
    public class ProductImageViewModel
    {
        public string Url { get; set; } 

        public int Width { get; set; }

        public int Height { get; set; }

        public int Type { get; set; } 

        public uint Identity { get; set; }
    }

    public class ProductPriceViewModel
    {
        public string Id { get; set; }

        public float Original { get; set; }

        public float Present { get; set; }

        public float Membership { get; set; }
    }

    public class ProductCreateViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public List<ProductImageViewModel> Images { get; set; }

        public string Title { get; set; }

        public string Introduction { get; set; }

        public string Details { get; set; }

        public ProductPriceViewModel Prices { get; set; }

        public int Type { get; set; }

        public int Group { get; set; }

        public int PubType { get; set; }

        public bool CanCollection { get; set; }

        public DateTime ResDateStart { get; set; }

        public DateTime ResDateEnd { get; set; }

        public string Widget { get; set; }
    }

    public class ProductSearchViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}