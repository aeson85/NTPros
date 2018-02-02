using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext;
        private Hashtable _repositories;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IRepository<T> Repository<T>() where T : class, IBaseEntity
        {
            _repositories = _repositories ?? new Hashtable();
            var genericType = typeof(T);
            var genericTypeTypeName = Regex.Replace(genericType.Name, "_", "");
            IRepository<T> repository = null;
            if (!_repositories.ContainsKey(genericTypeTypeName))
            {
                var type = this.GetType().Assembly.GetTypes().Single(p => p.Name.Equals($"{genericTypeTypeName}Repository", StringComparison.OrdinalIgnoreCase));
                repository = Activator.CreateInstance(type, _dbContext) as IRepository<T>;
                _repositories.Add(genericTypeTypeName, repository);
            }
            return _repositories[genericTypeTypeName] as IRepository<T>;
        }
    }
}