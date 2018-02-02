using System.Collections.Generic;

namespace NT_Test
{
    public class Handler
    {
        private readonly IUnitOfWork _unitOfWork;

        public Handler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public void NewAuthor()
        {
            _unitOfWork.Repository<Author>().Add(new Author {
                ID = 100,
                Name = "杨希玥1",
                Books = new List<Book> 
                {
                    new Book 
                    {
                        Title = "人之初，性本善"
                    },
                    new Book 
                    {
                        Title = "性相近，习相远"
                    }
                }
            });
        }

        public void Commit()
        {
            _unitOfWork.Commit();
        }
    }
}