namespace NT_Test
{
    public class Book
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public int Author_ID { get; set; } 
        
        public virtual Author Author { get; set; }
    }
}