namespace NT_Test
{
    public class BookRepository : GenericRepository<Book>
    {
        public BookRepository(AppDbContext context) : base(context)
        {
        }
    }
}