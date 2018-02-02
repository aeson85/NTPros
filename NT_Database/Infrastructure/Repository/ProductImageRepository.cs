using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class ProductImageRepository : GenericRepository<Product_Image>
    {
        public ProductImageRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}