using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NT_WebApp.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Product> Product { get; set; }

        public DbSet<NTImage> NTImage { get; set; }

        public DbSet<NTPrice> NTPrice { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        
        public static async Task CreateAdminAccount(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var name = configuration["Data:AdminUser:Name"];
            var email = configuration["Data:AdminUser:Email"];
            var password = configuration["Data:AdminUser:Password"];
            var roleName = configuration["Data:AdminUser:Role"];
            if (await userManager.FindByNameAsync(name) == null)
            {
                if (await roleManager.FindByNameAsync(roleName) == null)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                var adminUser = new AppUser
                {
                    UserName = name,
                    Email = email
                };
                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, roleName);
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //this.UseDbSetNamesAsTableNames(builder);
            
            builder.Entity<IdentityUser<string>>().ToTable("AspNetUsers").Property(p => p.Id).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityRole<string>>().ToTable("AspNetRoles").Property(p => p.Id).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserLogin<string>>().Property(p => p.ProviderKey).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserLogin<string>>().Property(p => p.LoginProvider).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserToken<string>>().Property(p => p.LoginProvider).HasMaxLength(256).IsRequired();

            builder.Entity<IdentityUserToken<string>>().Property(p => p.Name).HasMaxLength(256).IsRequired();

            builder.Entity<Product_Price>().HasKey(nameof(Product_Price.ProductId), nameof(Product_Price.PriceId));

            builder.Entity<Product>().HasOne(p => p.Product_Price).WithOne(p => p.Product).HasForeignKey<Product_Price>(p => p.ProductId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product_Image>().HasKey(nameof(Product_Image.ProductId), nameof(Product_Image.ImageId));

            builder.Entity<Product>().HasMany(p => p.Product_Image_Lst).WithOne(p => p.Product).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AppUser>().HasOne(p => p.WeChatInfo).WithOne(p => p.Owner).OnDelete(DeleteBehavior.Cascade);
        }
    }
}