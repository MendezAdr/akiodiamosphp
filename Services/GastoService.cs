using CrudCafeteria.Data;
using CrudCafeteria.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudCafeteria.Services
{
    public class GastoService
    {   
        //inyeccion del context
        private readonly CafeteriaContext _context;

        public GastoService(CafeteriaContext context)
        {
            _context = context;
        }

        // obtener todos los gastos
        public async Task<List<Gasto>> GetGastosAsync()
        {
            
            return await _context.Gastos
                .OrderByDescending(g => g.Fecha)
                .ToListAsync(); //retorna una lista
        }

        //obtener un solo gasto. maneja nulos
        public async Task<Gasto?> GetGastoByIdAsync(int id)
        {
            
            return await _context.Gastos.FindAsync(id);
        }
        // crear gasto
        public async Task<Gasto> CreateGastoAsync(Gasto gasto)
        {
            
            _context.Gastos.Add(gasto);
            await _context.SaveChangesAsync();
            return gasto; //devuelve el objeto para manejar una notificacion
        }

        // aqui se definen las respuestas que se van a manejar en el controlador
        // Succes == 200
        // NotFound == 404
        // ConcurrencyError == 409. es un error de sincronia
        public enum UpdateResult { Success, NotFound, ConcurrencyError }

        //modificar gasto
        public async Task<UpdateResult> UpdateGastoAsync(int id, Gasto gasto)
        {
            if (id != gasto.Id)
            {
                
                throw new ArgumentException("El ID de la URL no coincide con el ID del gasto.");
            }

            // 1. Busca el Gasto original que el Context ya está rastreando
            var gastoDB = await _context.Gastos.FindAsync(id);

            // 2. Verifica si existe
            if (gastoDB == null)
            {
                return UpdateResult.NotFound;
            }

            gastoDB.Descripcion = gasto.Descripcion;
            gastoDB.Monto = gasto.Monto;
            gastoDB.Categoria = gasto.Categoria;
            gastoDB.TipoGasto = gasto.TipoGasto;
            gastoDB.Fecha = gasto.Fecha;
            gastoDB.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return UpdateResult.Success; // exitoso
            }
            catch (DbUpdateConcurrencyException)
            {
                
                if (!_context.Gastos.Any(e => e.Id == id))
                {
                    return UpdateResult.NotFound; // 404
                }
                else
                {
                    //excepcion diferente
                    throw; 
                }
            }
        }

        //borrar gasto
        public async Task<bool> DeleteGastoAsync(int id)
        {
            // al carajo el gasto jajajaja ayudaestoycansado 
            var gasto = await _context.Gastos.FindAsync(id);
            if (gasto == null)
            {
                // si no existe, devuelve 'false'
                return false; 
            }

            _context.Gastos.Remove(gasto);
            await _context.SaveChangesAsync();
            
            // devuelve 'true' si se eliminó
            return true;
        }
    }
}