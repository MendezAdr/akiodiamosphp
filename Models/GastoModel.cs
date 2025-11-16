namespace CrudCafeteria.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Gasto
{   
    [Key]
    public int Id { get; set; }


    //descripcion
    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 100 caracteres.")]

    public string Descripcion { get; set; } = string.Empty;

    //monto
    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor que cero.")]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Monto { get; set; }

    //categoria
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "La categoría debe tener entre 3 y 50 caracteres.")]
    public string Categoria { get; set; } = string.Empty;

    //fecha
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }


    //usuario id
    [Column("user_id")]
    public int? UsuarioId { get; set; }

    //creado
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    //actualizado
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    //tipo de gasto
    // arreglado, ahora no es un tipo fijo. menos mal
    [Required(ErrorMessage = "El tipo de gasto es obligatorio.")] 
    [Column("tipo_gasto")]
    public string TipoGasto { get; set; } = string.Empty;

}