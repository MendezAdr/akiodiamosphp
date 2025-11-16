using CrudCafeteria.Data;
using CrudCafeteria.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using BC = BCrypt.Net.BCrypt;


namespace CrudCafeteria.Services
{
    public class UsuarioService
    {   
        //inyeccion de context
        private readonly CafeteriaContext _context;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(CafeteriaContext context, ILogger<UsuarioService> logger)
        {
            _context = context;
            _logger = logger;
        }
        // obtener todos los usuarios
        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            
            return await _context.Usuarios.ToListAsync(); //lista
        }
        // obtener un usuario
        public async Task<Usuario?> GetUsuarioByIdAsync(int id)
        {
            Console.WriteLine($"metodo del servicio llamado para buscar usuario: {id} ");
            return await _context.Usuarios.FindAsync(id);
        }
        // crear usuario
        public async Task<Usuario> CreateUsuarioAsync(Usuario usuario)
        {   

            
            // Hashear la contraseña con BCrypt antes de guardarla
            usuario.Password = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
        // declaracion de los errores
        public enum UpdateResult { Success, NotFound, ConcurrencyError }

        // actualizar usuario
        public async Task<UpdateResult> UpdateUsuarioAsync(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                // Lógica movida desde el controlador
                throw new ArgumentException("El ID de la URL no coincide con el ID del usuario.");
            }
            
            // agregar seccion para alterar la contraseña aqui
            
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return UpdateResult.Success;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Lógica movida desde el controlador
                if (!_context.Usuarios.Any(e => e.Id == id))
                {
                    return UpdateResult.NotFound;
                }
                else
                {
                    throw;
                }
            }
        }
        
        // borrar usuario
        public async Task<bool> DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return false; // No se encontró
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return true; // Se eliminó
        }
    
        //validar usuario (loooogin)
        // UsuarioService.cs

        public async Task<Usuario?> ValidarUsuarioAsync(string username, string password)
        {
            _logger.LogInformation("Servicio ValidarUsuarioAsync buscando a: {Username}", username);
            Console.WriteLine($"se llama al metodo para obtener el usuario: {username}");


            var usuario = await _context.Usuarios
                .Include(u => u.Rol) // Carga el Rol
                .FirstOrDefaultAsync(u => u.Username == username);

            if (usuario != null)
            {
                _logger.LogInformation("Usuario encontrado. ID: {UserId}. Hash en BD: {Hash}", usuario.Id, usuario.Password);

                // --- 3. Usamos el alias "BC" ---
                if (BC.Verify(password, usuario.Password))
                {
                    _logger.LogInformation("¡BCrypt.Verify TUVO ÉXITO!");
                    Console.WriteLine("¡BCrypt.Verify TUVO ÉXITO!");
                    return usuario; // ¡Éxito!
                }
                else
                {   
                    Console.WriteLine("¡BCrypt.Verify FALLÓ! (Contraseña incorrecta)");
                    _logger.LogWarning("¡BCrypt.Verify FALLÓ! (Contraseña incorrecta)");
                }
            }
            else
            {
                _logger.LogWarning("No se encontró ningún usuario con el nombre: {Username}", username);
            }
            return null; // Error
        }

        public async Task<Usuario?> GetUsuarioByUsernameAsync(string username)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    
    }
}