namespace NT_Test.ABC
{
    public class AuthorRepository : GenericRepository<Author>
    {
        public AuthorRepository(AppDbContext context) : base(context)
        {
        }
    }
}