using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System;

namespace NT_Model.Entity
{
    public class Product : IBaseEntity
    {
        [MaxLength(256)]
        public string Id { get; set; }

        public string Name { get; set; }

        private List<Product_Image> _product_Image_lst;
        public List<Product_Image> Product_Image_Lst
        {
            get
            {
                return _product_Image_lst;
            }
            set
            {
                _product_Image_lst = value;
            }
        }
        

        public string Title { get; set; }

        public string Introduction { get; set; }

        public string Details { get; set; }

        public Product_Price Product_Price { get; set; }

        public int Type { get; set; }

        public int Group { get; set; }

        public int PubType { get; set; }

        public bool CanCollection { get; set; }

        public DateTime ResDateStart { get; set; }

        public DateTime ResDateEnd { get; set; }

        public string Widget { get; set; }
    }
}