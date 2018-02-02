using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NT_Model.Entity;
using System;

namespace NT_Database.Infrastructure.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class, IBaseEntity;
        
        void Commit();
    }
}