using AutoMapper;
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
using NT_Common.Extensions;
using NT_Model.Entity;
using NT_Model.ViewModel;

namespace NT_WebApp.Infrastructure
{
    public class FTPProfile : Profile
    {
        public FTPProfile(IConfiguration configuration)
        {
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