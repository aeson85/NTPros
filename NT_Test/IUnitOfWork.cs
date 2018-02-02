using System;

namespace NT_Test
{
    public interface IUnitOfWork
    {
        //IRepository<Author> AuthorRepository { get; }

        IRepository<Book> BookRepository { get; }

        IRepository<T> Repository<T>() where T : class;

        void Commit();

        void RejectChanges();
    }
}