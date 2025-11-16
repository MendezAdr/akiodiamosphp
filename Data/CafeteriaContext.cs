using Microsoft.EntityFrameworkCore;
using CrudCafeteria.Models;

// Ajusta según tu namespace

namespace CrudCafeteria.Data
{
    public class CafeteriaContext : DbContext
    {
        public CafeteriaContext(DbContextOptions<CafeteriaContext> options) 
            : base(options)
        {
        }

        //esto va a conectar cada modelo con la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Ingreso> Ingresos { get; set; }

        public DbSet<Rol> Roles { get; set; } // <-- 1. AÑADE ESTA LÍNEA

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Usuario>().ToTable("users"); 
            modelBuilder.Entity<Gasto>().ToTable("gastos");  
            modelBuilder.Entity<Ingreso>().ToTable("ingresos");
            modelBuilder.Entity<Rol>().ToTable("Rol");


            // esto es para definir las propiedades de "usuario"
            modelBuilder.Entity<Usuario>(entity =>
            {
                // evita que el nombre de usuario se repita
                entity.HasIndex(e => e.Username).IsUnique();

                // lo mismo con el correo
                entity.HasIndex(e => e.Email).IsUnique();

                // fecha de creacion por defecto. usa la fecha de base de datos
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // estas son las de ingreso
            modelBuilder.Entity<Ingreso>(entity =>
            {   
                //hace redondea el monto a dos decimales y que no sea mas largo que 10 caracteres
                entity.Property(e => e.Monto)
                    .HasColumnType("decimal(10,2)");

                //fecha por defecto
                entity.Property(e => e.Fecha)
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
        }
    }
}