using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EstudianteRepository : IEstudianteRepository
    {
        private readonly ApplicationDbContext _context;

        public EstudianteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Estudiante>> GetAllAsync(string? filtro = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Estudiantes.Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToLower();
                query = query.Where(e =>
                    (e.Nombre != null && e.Nombre.ToLower().Contains(filtro)) ||
                    (e.Apellido != null && e.Apellido.ToLower().Contains(filtro)) ||
                    (e.Matricula != null && e.Matricula.ToLower().Contains(filtro))
                );
            }

            return await query
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? filtro = null)
        {
            var query = _context.Estudiantes.Where(e => e.Activo);

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToLower();
                query = query.Where(e =>
                    (e.Nombre != null && e.Nombre.ToLower().Contains(filtro)) ||
                    (e.Apellido != null && e.Apellido.ToLower().Contains(filtro)) ||
                    (e.Matricula != null && e.Matricula.ToLower().Contains(filtro))
                );
            }

            return await query.CountAsync();
        }

        public async Task<Estudiante?> GetByIdAsync(int id)
        {
            return await _context.Estudiantes.FindAsync(id);
        }

        public async Task AddAsync(Estudiante estudiante)
        {
            await _context.Estudiantes.AddAsync(estudiante);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Estudiante estudiante)
        {
            _context.Estudiantes.Update(estudiante);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Estudiante estudiante)
        {
            _context.Estudiantes.Remove(estudiante);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteMatriculaAsync(string matricula, int? excludeId = null)
        {
            var query = _context.Estudiantes.Where(e => e.Matricula == matricula.Trim().ToUpper());
            if (excludeId.HasValue)
                query = query.Where(e => e.EstudianteID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> TieneInscripcionesAsync(int estudianteId)
        {
            return await _context.Inscripciones.AnyAsync(i => i.EstudianteID == estudianteId);
        }

        public async Task<bool> TieneCalificacionesAsync(int estudianteId)
        {
            return await _context.Calificaciones.AnyAsync(c => c.Inscripcion.EstudianteID == estudianteId);
        }
    }
}
