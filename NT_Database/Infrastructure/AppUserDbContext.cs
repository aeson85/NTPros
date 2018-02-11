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
    public class AppUserDbContext : IdentityDbContext<AppUser>
    {
        public AppUserDbContext(DbContextOptions<AppUserDbContext> options) : base(options)
        {
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

            builder.Entity<AppUser>().HasOne(p => p.WeChatInfo).WithOne(p => p.Owner).OnDelete(DeleteBehavior.Cascade);
        }
    }
}