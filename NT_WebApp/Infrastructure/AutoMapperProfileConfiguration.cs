using AutoMapper;
using NT_WebApp.Models;
using NT_WebApp.Models.ViewModels;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.FileProviders.Physical;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace NT_WebApp.Infrastructure
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration(IConfiguration configuration)
        {
            this.CreateMap<ProductImageViewModel, NTImage>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));
            this.CreateMap<NTImage, ProductImageViewModel>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));;
            this.CreateMap<ProductPriceViewModel, NTPrice>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));;
            this.CreateMap<NTPrice, ProductPriceViewModel>().ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));;
            this.CreateMap<ProductCreateViewModel, Product>().ForMember(p => p.Product_Image_Lst, opt => 
            {
                opt.ResolveUsing<List<Product_Image>>((s, d, m, c) => 
                {
                    List<Product_Image> pro_img_lst = null;
                    if (s.Images?.Count() > 0)
                    {
                        pro_img_lst = new List<Product_Image>();
                        foreach (var item in s.Images)
                        {
                            var pro_img = m?.SingleOrDefault(p => p.Type == item.Type) ?? new Product_Image();
                            var img = pro_img.Image ?? new NTImage();
                            pro_img.Image = c.Mapper.Map(item, img);
                            pro_img.Type = item.Type;
                            pro_img_lst.Add(pro_img);
                        }
                    }
                    return pro_img_lst;
                });
            }).ForMember(p => p.Product_Price, opt =>
            {
                opt.ResolveUsing<Product_Price>((s, d, m, c) =>
                {
                    Product_Price pro_price = null;
                    if (s.Prices != null)
                    {
                        pro_price = m != null ? m as Product_Price : new Product_Price();
                        var price = pro_price.Price ?? new NTPrice(); 
                        pro_price.Price = c.Mapper.Map(s.Prices, price);
                    }
                    return pro_price;
                });
            }).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));;

            this.CreateMap<Product, ProductCreateViewModel>().ForMember(p => p.Images, opt => 
            {
                opt.ResolveUsing<List<ProductImageViewModel>>((s, d, m, c) => 
                {
                    if (s.Product_Image_Lst?.Count() > 0)
                    {
                        m = new List<ProductImageViewModel>();
                        foreach (var item in s.Product_Image_Lst)
                        {
                            var imgModel = c.Mapper.Map<ProductImageViewModel>(item.Image);
                            imgModel.Type = item.Type;
                            m.Add(imgModel);
                        } 
                    }
                    return m;
                });
            }).ForMember(p => p.Prices, opt => 
            {
                opt.ResolveUsing<ProductPriceViewModel>((s, d, m, c) => 
                {
                    if (s.Product_Price != null)
                    {
                        m = c.Mapper.Map<ProductPriceViewModel>(s.Product_Price.Price);
                    }
                    return m;
                });
            }).ForAllMembers(p => p.Condition((s, d, sm, dm) => sm != null));

            this.CreateMap<PhysicalDirectoryInfo, PhysicalDirectoryInfoViewModel>();
            this.CreateMap<PhysicalFileInfo, PhysicalDirectoryInfoViewModel>();
            this.CreateMap<IFileInfo, IPhysicalDirectoryInfoViewModel>().Include<PhysicalDirectoryInfo, PhysicalDirectoryInfoViewModel>().Include<PhysicalFileInfo, PhysicalDirectoryInfoViewModel>().ForMember(p => p.Url, opt => 
            {
                opt.ResolveUsing<string>((s, d, m, c) =>
                {
                    string url = null;
                    if (!s.IsDirectory)
                    {
                        url = s.PhysicalPath.Replace(@"\","/").Replace(configuration["Ftp:RootPath"], configuration["Ftp:Prefix"]);
                    }
                    return url;
                });
            });
        }
    }
}