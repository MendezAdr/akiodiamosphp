using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudCafeteria.Models
{
    [Table("Rol")] // Mapea a tu tabla 'Rol'
    public class Rol
    {
        [Key]
        public int Id { get; set; }

        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Column("permisos")]
        public string? Permisos { get; set; }

        [Column("descripcion")]
        public string? Descripcion { get; set; }
    }
}