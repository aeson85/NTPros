using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using NT_Model.ViewModel;
using NT_Model.Entity;
using NT_Common.Extensions;

namespace NT_CommonConfig.Infrastructure
{
    public class DbEntityProfile : Profile
    {
        public DbEntityProfile(IConfiguration configuration)
        {
            this.CreateMap<ProductImageViewModel, NTImage>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<NTImage, ProductImageViewModel>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<ProductPriceViewModel, NTPrice>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<NTPrice, ProductPriceViewModel>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<NTImage, NTImage>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<NTPrice, NTPrice>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<ProductSearchViewModel, Product>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<Product_Image, Product_Image>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<Product_Price, Product_Price>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));

            this.CreateMap<Product, Product>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            /*
            this.CreateMap<Product, Product>().ForMember(p => p.Product_Image_Lst, opt => 
            {
                opt.ResolveUsing((s, d, dm, c) => 
                {
                    List<Product_Image> pro_img_lst = new List<Product_Image>();
                    if (s.Product_Image_Lst?.Count() > 0)
                    {
                        foreach (var pro_img in s.Product_Image_Lst)
                        {
                            var dbPro_Img = d.Product_Image_Lst?.SingleOrDefault(p => p.MockId == pro_img.MockId);
                            if (dbPro_Img == null)
                            {
                                dbPro_Img = new Product_Image();
                            }
                            dbPro_Img.Image = dbPro_Img.Image ?? new NTImage();
                            dbPro_Img.Image = c.Mapper.Map(pro_img.Image, dbPro_Img.Image);
                            //dbPro_Img = c.Mapper.Map(pro_img, dbPro_Img);
                            pro_img_lst.Add(dbPro_Img);
                        }
                    }
                    return pro_img_lst;
                });
            }).ForMember(p => p.Product_Price, opt =>
            {
                opt.ResolveUsing<Product_Price>((s, d, dm, c) =>
                {
                    Product_Price pro_price = null;
                    if (s.Product_Price != null)
                    {
                        pro_price = s.Product_Price;
                        var price = pro_price.Price ?? new NTPrice(); 
                        pro_price.Price = price;
                    }
                    return pro_price;
                });
            }).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            */

            this.CreateMap<ProductCreateViewModel, Product>().ForMember(p => p.Product_Image_Lst, opt => 
            {
                opt.ResolveUsing<List<Product_Image>>((s, d, dm, c) => 
                {
                    List<Product_Image> pro_img_lst = new List<Product_Image>();
                    if (s.Images?.Count() > 0)
                    {
                        foreach (var item in s.Images)
                        {
                            var pro_img = dm?.FirstOrDefault(p => $"{p.ProductId}{p.ImageId}".ToCrc32().ToString() == item.MockId) ?? new Product_Image();
                            var img = pro_img.Image ?? new NTImage();
                            pro_img.Image = c.Mapper.Map(item, img);
                            pro_img.Type = item.Type;
                            pro_img.MockId = item.MockId;
                            pro_img_lst.Add(pro_img);
                        }
                    }
                    return pro_img_lst;
                });
            }).ForMember(p => p.Product_Price, opt =>
            {
                opt.ResolveUsing<Product_Price>((s, d, dm, c) =>
                {
                    Product_Price pro_price = null;
                    if (s.Prices != null)
                    {
                        pro_price = dm != null ? dm as Product_Price : new Product_Price();
                        var price = pro_price.Price ?? new NTPrice(); 
                        pro_price.Price = c.Mapper.Map(s.Prices, price);
                    }
                    return pro_price;
                });
            }).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));

            this.CreateMap<Product, ProductCreateViewModel>().ForMember(p => p.Images, opt => 
            {
                opt.ResolveUsing<List<ProductImageViewModel>>((s, d, dm, c) => 
                {
                    if (s.Product_Image_Lst?.Count() > 0)
                    {
                        dm = new List<ProductImageViewModel>();
                        foreach (var item in s.Product_Image_Lst)
                        {
                            var imgModel = c.Mapper.Map<ProductImageViewModel>(item.Image);
                            imgModel.Type = item.Type;
                            imgModel.MockId = $"{item.ProductId}{item.ImageId}".ToCrc32().ToString();
                            dm.Add(imgModel);
                        } 
                    }
                    return dm;
                });
            }).ForMember(p => p.Prices, opt => 
            {
                opt.ResolveUsing<ProductPriceViewModel>((s, d, dm, c) => 
                {
                    if (s.Product_Price != null)
                    {
                        dm = c.Mapper.Map<ProductPriceViewModel>(s.Product_Price.Price);
                        dm.MockId = $"{s.Product_Price.ProductId}{s.Product_Price.PriceId}".ToCrc32().ToString();
                    }
                    return dm;
                });
            }).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null)); 
        }
    }
}