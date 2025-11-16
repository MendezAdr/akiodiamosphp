using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudCafeteria.Models; 
using CrudCafeteria.Services; 

namespace CrudCafeteria.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GastosController : ControllerBase
    {
        private readonly GastoService _gastoService;

        public GastosController(GastoService gastoService)
        {
            _gastoService = gastoService;
        }

        
        // obtener todos los gastos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Gasto>>> GetManyGastos()
        {
            // El controlador solo llama al servicio
            var gastos = await _gastoService.GetGastosAsync();
            return Ok(gastos); // Devuelve 200 OK con la lista
        }

        
        // obtener un gasto
        [HttpGet("{id}")]
        public async Task<ActionResult<Gasto>> GetGasto(int id)
        {
            var gasto = await _gastoService.GetGastoByIdAsync(id);

            if (gasto == null)
            {
                // 404
                return NotFound(); 
            }

            return Ok(gasto); 
        }

        
        // Crear a nivel de controlador
        [HttpPost]
        public async Task<ActionResult<Gasto>> PostGasto(Gasto gasto)
        {
            var gastoCreado = await _gastoService.CreateGastoAsync(gasto);
            
            // gasto creado
            return CreatedAtAction(nameof(GetGasto), new { id = gastoCreado.Id }, gastoCreado);
        }

        // editar gasto a nivel de controlador
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGasto(int id, Gasto gasto)
        {   
            try
            {
                var resultado = await _gastoService.UpdateGastoAsync(id, gasto);

                if (resultado == GastoService.UpdateResult.NotFound)
                {
                    return NotFound(); // 404 
                }
            }
            catch (ArgumentException ex)
            {
                
                return BadRequest(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                // manda el error de asincronia
                return Conflict("Error de concurrencia. El registro fue modificado por otro usuario.");
            }

            return NoContent(); // 204
        }

        
        // Borrar en controlador
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGasto(int id)
        {   
            var resultado = await _gastoService.DeleteGastoAsync(id);

            if (!resultado)
            {
                // 404
                return NotFound();
            }

            return NoContent(); // 204
        }
    }
}