using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NT_Model.Entity;

namespace NT_Database.Infrastructure.Repository
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _dbContext;
        private readonly AppUserDbContext _userDbContext;
        private readonly IServiceProvider _serviceProvider;

        private Hashtable _repositories;

        public UnitOfWork(AppDbContext dbContext, AppUserDbContext userDbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _userDbContext = userDbContext;
            _serviceProvider = serviceProvider;
        }

        public void Commit()
        {
            _dbContext.SaveChanges();
            _userDbContext.SaveChanges();
        }

        public void Dispose()
        {
            //_dbContext.Dispose();
            //_userDbContext.Dispose();
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
                repository = _serviceProvider.GetRequiredService(type) as IRepository<T>;
                _repositories.Add(genericTypeTypeName, repository);
            }
            return _repositories[genericTypeTypeName] as IRepository<T>;
        }
    }
}