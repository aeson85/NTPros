using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace NT_Test
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        private Hashtable _repositories;

        public UnitOfWork(AppDbContext dbContext) => _dbContext = dbContext;

        public UnitOfWork()
        {
        }

        //public IRepository<Author> AuthorRepository => new AuthorRepository(_dbContext);

        public IRepository<Book> BookRepository => new BookRepository(_dbContext);

        public IRepository<T> Repository<T>() where T : class
        {
            _repositories = _repositories ?? new Hashtable();
            var genericType = typeof(T);
            var genericTypeTypeName = genericType.Name;
            IRepository<T> repository = null;
            if (!_repositories.ContainsKey(genericTypeTypeName))
            {
                var type = typeof(T).Assembly.GetTypes().Single(p => p.Name.Equals($"{genericTypeTypeName}Repository"));
                repository = Activator.CreateInstance(type, _dbContext) as IRepository<T>;
                _repositories.Add(genericTypeTypeName, repository);
            }
            return _repositories[genericTypeTypeName] as IRepository<T>;
        }
        
        public void Commit()
        {
            _dbContext.SaveChanges();
        }

        public void RejectChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}