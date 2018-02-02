using Microsoft.EntityFrameworkCore;

namespace NT_Test
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Author { get; set; }

        public DbSet<Book> Book { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
            
        }
    }
}