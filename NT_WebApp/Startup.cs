using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NT_WeChatUtilities;
using NT_WebApp.Infrastructure;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using NT_CommonConfig.Infrastructure;
using NT_Model.Entity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace NT_WebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

        public readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddDbContext<AppDbContext>(opts =>
            // {
            //     opts.UseMySQL(_configuration["Database:ConnectionString"]);
            // });
            // services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // services.AddIdentity<AppUser, IdentityRole>(opt => 
            // {
            //     opt.Password.RequiredLength = 6;
            //     opt.Password.RequireNonAlphanumeric = false;
            //     opt.Password.RequireLowercase = false;
            //     opt.Password.RequireUppercase = false;
            //     opt.Password.RequireDigit = false;
            //     opt.User.RequireUniqueEmail = true;
            // });

            services.AddAuthentication(opt =>
            {
                opt.DefaultChallengeScheme = "oidc";
                opt.DefaultScheme = "Cookies";
            }).AddCookie("Cookies").AddOpenIdConnect("oidc", opt =>
            {
                opt.SignInScheme = "Cookies";
                opt.Authority = _configuration["AuthServer:Host"];
                opt.RequireHttpsMetadata = false;
                opt.ClientId = "mvc";
                opt.ClientSecret = "secret";
                opt.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                opt.GetClaimsFromUserInfoEndpoint = true;
                opt.SaveTokens = true;
                opt.Authority = "http://localhost:5001";
                opt.Scope.Add("msgpublish_api");
                opt.Scope.Add("offline_access");
            });
            var physicalFileProvider = new PhysicalFileProvider(_configuration["Ftp:RootPath"]);
            services.AddSingleton<IFileProvider>(physicalFileProvider);

            // services.AddDistributedRedisCache(opt => 
            // {
            //     opt.Configuration = this.Configuration["Redis:Host"];
            //     opt.InstanceName = this.Configuration["Redis:InstanceName"];
            // });

            services.AddSingleton<MQPublishServerUrls>();
            services.AddSingleton<WeChatApiUrls>();
            services.AddSingleton<WeChatUtilities>();
            services.AddUrlHelper();
        
            services.AddSingleton(provider => new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new DbEntityProfile(_configuration));
                cfg.AddProfile(new FTPProfile(_configuration));
            }).CreateMapper());

            services.AddSingleton(factory => 
            {
                Func<string, MessageDelivery> accessor = key => 
                {
                    switch (key)
                    {
                        case "wechat": return new WeChatMessageDelivery(_configuration, factory.GetRequiredService<WeChatUtilities>(), factory.GetRequiredService<MQPublishServerUrls>());
                        default: return null;
                    }
                };
                return accessor;
            });

            services.AddMvc();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = app.ApplicationServices.GetRequiredService<IFileProvider>(),
                RequestPath = _configuration["Ftp:Prefix"]
            });

            app.UseCors(builder => builder.WithOrigins("http://localhost").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "",
                    template: "{action=index}",
                    defaults: new { controller = "home"}
                );

                routes.MapRoute(
                    name: "",
                    template: "{controller=admin}/{action=index}/{id?}"
                );
            });
            //AppDbContext.CreateAdminAccount(app.ApplicationServices, this.Configuration).Wait();
        }
    }
}
