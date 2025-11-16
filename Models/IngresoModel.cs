namespace CrudCafeteria.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Ingreso
{   
    [Key]
    public int Id { get; set; }

    //fecha

    [Required(ErrorMessage = "La fecha es obligatoria.")]
    public DateTime Fecha { get; set; }


    //concepto
    [Required(ErrorMessage = "La descripcion es obligatoria.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El concepto debe tener entre 3 y 100 caracteres.")]
    public string Descripcion { get; set; } = string.Empty;

    //monto
    [Required(ErrorMessage = "El monto es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser un valor positivo.")]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Monto { get; set; }

    //por ahora dejamos venta como tipo de ingreso
    //a futuro hay que modificarlo. QUE NO SE ME OLVIDE NOJODA!!
    [Required]
    public string TipoIngreso { get; set; } = "venta";

    //usuario
    //el usuario no es precisamente requerido. pa que si esta en base de datos?
    //creo que estoy diciendo algo sin sentido
    public int? UsuarioId { get; set; }


    //este campo es opcional
    [StringLength(255, ErrorMessage = "Las observaciones no pueden exceder los 255 caracteres.")]
    public string? Observaciones { get; set; }
    
    //fechas
    
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    
}