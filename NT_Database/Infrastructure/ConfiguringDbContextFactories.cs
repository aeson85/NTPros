using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NT_CommonConfig.Infrastructure;

namespace NT_Database.Infrastructure
{
    public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = ConfigurationSettings.Initial(Directory.GetCurrentDirectory()).Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseMySQL(config["Database:ConnectionString"]);
            return new AppDbContext(builder.Options);
        }
    }

    public class DesignTimeAppUserDbContextFactory : IDesignTimeDbContextFactory<AppUserDbContext>
    {
        public AppUserDbContext CreateDbContext(string[] args)
        {
            var config = ConfigurationSettings.Initial(Directory.GetCurrentDirectory()).Build();

            var builder = new DbContextOptionsBuilder<AppUserDbContext>();
            builder.UseMySQL(config["Database:UserConnectionString"]);
            return new AppUserDbContext(builder.Options);
        }
    }
}