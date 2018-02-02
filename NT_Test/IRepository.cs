using System.Linq;

namespace NT_Test
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Entities { get; }

        void Remove(T entity);
        
        void Add(T entity);
    }
}