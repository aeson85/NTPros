using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class NTPriceRepository : GenericRepository<NTPrice>
    {
        public NTPriceRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}