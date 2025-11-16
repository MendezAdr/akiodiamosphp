using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudCafeteria.Models;
using CrudCafeteria.Services; 

namespace CrudCafeteria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly UsuarioService _usuarioService;

        public UserController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // obtener todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetManyUsuarios()
        {
            var usuarios = await _usuarioService.GetUsuariosAsync();
            return Ok(usuarios);
        }

        // obtener un usuario
        [HttpGet("{id}")]
        // --- CORRECCIÓN 1: Devolvía 'Gasto', debe devolver 'Usuario' ---
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            // --- CORRECCIÓN 2: Variable 'gasto' renombrada a 'usuario' ---
            var usuario = await _usuarioService.GetUsuarioByIdAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // Subir un usuario
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            // ojo con el hasheo aqui
            var usuarioCreado = await _usuarioService.CreateUsuarioAsync(usuario);
            
            return CreatedAtAction(nameof(GetUsuario), new { id = usuarioCreado.Id }, usuarioCreado);
        }

        // actualiza
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            try
            {
                var resultado = await _usuarioService.UpdateUsuarioAsync(id, usuario);

                // --- CORRECCIÓN 3: Era 'GastoService.UpdateResult' ---
                if (resultado == UsuarioService.UpdateResult.NotFound)
                {
                    return NotFound();
                }
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Error de concurrencia. El registro fue modificado por otro usuario.");
            }

            return NoContent();
        }

        // borrar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var resultado = await _usuarioService.DeleteUsuarioAsync(id);

            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}