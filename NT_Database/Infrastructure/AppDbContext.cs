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
using NT_Model.Entity;

namespace NT_Database.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Product { get; set; }

        public DbSet<NTImage> NTImage { get; set; }

        public DbSet<Product_Image> Product_Image { get; set; }

        public DbSet<NTPrice> NTPrice { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Product_Price>().HasKey(nameof(Product_Price.ProductId), nameof(Product_Price.PriceId));

            builder.Entity<Product>().HasOne(p => p.Product_Price).WithOne(p => p.Product).HasForeignKey<Product_Price>(p => p.ProductId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Product_Image>().HasKey(nameof(NT_Model.Entity.Product_Image.ProductId), nameof(NT_Model.Entity.Product_Image.ImageId));

            builder.Entity<Product>().HasMany(p => p.Product_Image_Lst).WithOne(p => p.Product).OnDelete(DeleteBehavior.Cascade);
        }
    }
}