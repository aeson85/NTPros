using Microsoft.EntityFrameworkCore;
using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class AppUserRepository : GenericRepository<AppUser>
    {
        public AppUserRepository(AppUserDbContext dbContext) : base(dbContext)
        {
        }
    }
}