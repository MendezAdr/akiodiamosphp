using CrudCafeteria.Data;
using CrudCafeteria.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudCafeteria.Services
{
    public class IngresoService
    {
        //inyeccion del context
        private readonly CafeteriaContext _context;

        public IngresoService(CafeteriaContext context)
        {
            _context = context;
        }
        // obtener todos los ingresos
        public async Task<List<Ingreso>> GetIngresosAsync()
        {
            
            return await _context.Ingresos
                .OrderByDescending(i => i.Fecha)
                .ToListAsync(); //retorna una lista
        }
        // obtener un ingreso
        public async Task<Ingreso?> GetIngresoByIdAsync(int id)
        {
            
            return await _context.Ingresos.FindAsync(id); //puede ser nulo
        }

        // crear un ingreso
        public async Task<Ingreso> CreateIngresoAsync(Ingreso ingreso)
        {
            
            _context.Ingresos.Add(ingreso);
            await _context.SaveChangesAsync();
            return ingreso;
        }

        // aqui se definen las respuestas que se van a manejar en el controlador
        // Succes == 200
        // NotFound == 404
        // ConcurrencyError == 409. es un error de sincronia
        public enum UpdateResult { Success, NotFound, ConcurrencyError }

        //actualizar ingreso
        public async Task<UpdateResult> UpdateIngresoAsync(int id, Ingreso ingreso)
        {
            if (id != ingreso.Id)
            {
                // error con la url
                throw new ArgumentException("El ID de la URL no coincide con el ID del ingreso.");
            }

            // para cambiar la fecha de actualizacion
            ingreso.UpdatedAt = DateTime.UtcNow; 
            
            _context.Entry(ingreso).State = EntityState.Modified; //cambia el estado para que se modifique

            //manejo de excepciones
            try
            {
                await _context.SaveChangesAsync();
                return UpdateResult.Success;// exitoso
            }
            catch (DbUpdateConcurrencyException)
            {
                // Lógica movida desde el controlador
                if (!_context.Ingresos.Any(e => e.Id == id))
                {
                    return UpdateResult.NotFound; // 404
                }
                else
                {   // error generico
                    throw;
                }
            }
        }

        //adios ingreso
        public async Task<bool> DeleteIngresoAsync(int id)
        {
            
            var ingreso = await _context.Ingresos.FindAsync(id);
            if (ingreso == null)
            {
                return false; // No se encontró
            }

            _context.Ingresos.Remove(ingreso);
            await _context.SaveChangesAsync();
            return true; // Se eliminó
        }
    }
}