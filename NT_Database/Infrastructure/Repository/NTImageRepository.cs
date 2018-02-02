using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class NTImageRepository : GenericRepository<NTImage>
    {
        public NTImageRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}