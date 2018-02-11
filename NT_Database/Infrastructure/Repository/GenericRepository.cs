using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NT_Database.Infrastructure;
using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public virtual IQueryable<TResult> Get<TResult>(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true)
        {
            var query = this.Get(predicate, orderBy, include, disableTracking);
            return query.ProjectTo<TResult>();
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (disableTracking)
            {
                query = query.AsNoTracking();
            }
            if (include != null)
            {
                query = include(query);
            }
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return query;
        }

        public IEnumerable<T> Local(params EntityState[] entityStates)
        {
            foreach (var item in _dbSet.Local.ToList())
            {
                if (entityStates.Distinct().Count(p => p == _dbContext.Entry(item).State) > 0)
                {
                    yield return item;
                }
            }
        }

        public virtual void Remove(string id)
        {
            var entity = this.SingleOrDefault(p => p.Id == id, disableTracking: false);
            this.Remove(entity);
        }

        public virtual void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(params T[] entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public virtual TResult SingleOrDefault<TResult>(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = false)
        {
            var query = this.Get<TResult>(predicate, orderBy, include, disableTracking);
            return query.SingleOrDefault<TResult>();
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null, bool disableTracking = false)
        {
            var query = this.Get(predicate, orderBy, include, disableTracking);
            return query.SingleOrDefault();
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
            //_dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}