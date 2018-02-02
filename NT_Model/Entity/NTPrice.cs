using System.ComponentModel.DataAnnotations;

namespace NT_Model.Entity
{
    public class NTPrice : IBaseEntity
    {
        [MaxLength(256)]
        public string Id { get; set; }

        public float Original { get; set; }

        public float Present { get; set; }

        public float Membership { get; set; }
    }
}