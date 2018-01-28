using System.ComponentModel.DataAnnotations;

namespace NT_Model.Entity
{
    public class NTImage
    {
        [MaxLength(256)]
        public string Id { get; set; }
        
        public string Url { get; set; } 

        public int Width { get; set; }

        public int Height { get; set; }
    }
}