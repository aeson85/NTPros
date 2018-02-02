using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class ProductRepository : GenericRepository<Product>
    {
        private Func<IQueryable<Product>, IIncludableQueryable<Product, object>> _defaultInclude;

        public ProductRepository(AppDbContext dbContext) : base(dbContext)
        {
            _defaultInclude = query => query.Include(p => p.Product_Image_Lst).ThenInclude(p => p.Image).Include(p => p.Product_Price).ThenInclude(p => p.Price);
        }
        
        public override IQueryable<Product> Get(Expression<Func<Product, bool>> predicate = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include = null, bool disableTracking = true) => base.Get(predicate, orderBy, include ?? _defaultInclude, disableTracking);

        public override IQueryable<TResult> Get<TResult>(Expression<Func<Product, bool>> predicate = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include = null, bool disableTracking = true) => base.Get<TResult>(predicate, orderBy, include ?? _defaultInclude, disableTracking);

        public override Product SingleOrDefault(Expression<Func<Product, bool>> predicate = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, Func<IQueryable<Product>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Product, object>> include = null, bool disableTracking = false) => base.SingleOrDefault(predicate, orderBy, include ?? _defaultInclude, disableTracking);

        public override TResult SingleOrDefault<TResult>(Expression<Func<Product, bool>> predicate = null, Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null, Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include = null, bool disableTracking = false) => base.SingleOrDefault<TResult>(predicate, orderBy, include ?? _defaultInclude, disableTracking);
    }
}