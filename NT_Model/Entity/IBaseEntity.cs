using System.ComponentModel.DataAnnotations;

namespace NT_Model.Entity
{
    public interface IBaseEntity
    {
        string Id { get; set; }
    }
}