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
    public class CalificacionRepository : ICalificacionRepository
    {
        private readonly ApplicationDbContext _context;

        public CalificacionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Calificacion>> GetByInscripcionAsync(int inscripcionId)
        {
            return await _context.Calificaciones
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .Where(c => c.InscripcionID == inscripcionId)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Calificacion>> GetByEstudianteAsync(int estudianteId)
        {
            return await _context.Calificaciones
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .Where(c => c.Inscripcion.EstudianteID == estudianteId)
                .OrderBy(c => c.FechaCreacion)
                .ToListAsync();
        }

        public async Task<Calificacion?> GetByIdAsync(int id)
        {
            return await _context.Calificaciones
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(c => c.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .FirstOrDefaultAsync(c => c.CalificacionID == id);
        }

        public async Task AddAsync(Calificacion calificacion)
        {
            await _context.Calificaciones.AddAsync(calificacion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Calificacion calificacion)
        {
            _context.Calificaciones.Update(calificacion);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Calificacion calificacion)
        {
            _context.Calificaciones.Remove(calificacion);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int inscripcionId, int? excludeId = null)
        {
            var query = _context.Calificaciones.Where(c => c.InscripcionID == inscripcionId);
            if (excludeId.HasValue)
                query = query.Where(c => c.CalificacionID != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
