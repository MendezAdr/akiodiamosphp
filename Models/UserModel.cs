namespace CrudCafeteria.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Usuario
{   
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "el nombre del usuario es obligatorio")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "el Email es obligatorio")]
    [EmailAddress(ErrorMessage = "el formato de Email no es valido")]
    public string Email { get; set; } = string.Empty;

    // --- CORRECCIÓN DE MAPEO ---
    [Column("fecha_nacimiento")]
    public DateTime FechaNacimiento { get; set; } // Antes: Fecha_nacimienti

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // --- CORRECCIÓN DE MAPEO (El error principal) ---
    [Required(ErrorMessage = "La contrasena es obligatoria")]
    [Column("password")] // Mapea a la columna 'password' de la BD
    public string Password { get; set; } = string.Empty; 

    // --- PROPIEDADES FALTANTES (Para el login) ---
    [Column("rol_id")]
    public int? RolId { get; set; } // Tu BD permite nulos

    // Propiedad de navegación (requiere un Models/Rol.cs)
    public virtual Rol? Rol { get; set; }
    // ---------------------------------------------

    [NotMapped] // No se guarda en la BD
    [Compare("Password", ErrorMessage = "las contrasenas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
