using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class ProductPriceRepository : GenericRepository<Product_Price>
    {
        public ProductPriceRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}