using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using NT_Common.Infrastructure;
using NT_Test;

namespace NT_Database.Infrastructure
{
    public class ConfiguringDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var config = ConfigurationSettings.Initial(null, null, Directory.GetCurrentDirectory()).Build();
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseMySQL("server=localhost;database=unitofworkdb;user=root;password=Cosmicworks123");
            return new AppDbContext(builder.Options);
        }
    }
}