using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudCafeteria.Models;
using CrudCafeteria.Services; 

namespace CrudCafeteria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngresosController : ControllerBase
    {
        private readonly IngresoService _ingresoService;

        public IngresosController(IngresoService ingresoService)
        {
            _ingresoService = ingresoService;
        }

        // obtener todos los ingresos a nivel de controlador
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ingreso>>> GetManyIngresos()
        {
            var ingresos = await _ingresoService.GetIngresosAsync();
            return Ok(ingresos);
        }

        // Obtener un ingreso
        [HttpGet("{id}")]
        public async Task<ActionResult<Ingreso>> GetIngreso(int id)
        {
            var ingreso = await _ingresoService.GetIngresoByIdAsync(id);

            if (ingreso == null)
            {
                return NotFound();
            }

            return Ok(ingreso); //204
        }

        // Subir un gasto
        [HttpPost]
        public async Task<ActionResult<Ingreso>> PostIngreso(Ingreso ingreso)
        {
            var ingresoCreado = await _ingresoService.CreateIngresoAsync(ingreso);
            
            return CreatedAtAction(nameof(GetIngreso), new { id = ingresoCreado.Id }, ingresoCreado);
        }

        // Actualizar
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIngreso(int id, Ingreso ingreso)
        {
            try
            {
                var resultado = await _ingresoService.UpdateIngresoAsync(id, ingreso);

                if (resultado == IngresoService.UpdateResult.NotFound)
                {
                    return NotFound();
                }
            }
            catch (ArgumentException ex)
            {
                // maneja el error
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict("Error de concurrencia. El registro fue modificado por otro usuario.");
            }

            return NoContent();
        }

        // Borrar
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngreso(int id)
        {
            var resultado = await _ingresoService.DeleteIngresoAsync(id);

            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}