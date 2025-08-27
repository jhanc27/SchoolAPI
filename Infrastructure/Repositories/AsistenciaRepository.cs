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
    public class AsistenciaRepository : IAsistenciaRepository
    {
        private readonly ApplicationDbContext _context;

        public AsistenciaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Asistencia>, int)> GetAsync(DateTime? fecha, int? inscripcionId, int page, int pageSize)
        {
            var query = _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion.Materia)
                .AsQueryable();

            if (fecha.HasValue)
                query = query.Where(a => a.Fecha.Date == fecha.Value.Date);

            if (inscripcionId.HasValue)
                query = query.Where(a => a.InscripcionID == inscripcionId.Value);

            var totalRecords = await query.CountAsync();
            var asistencias = await query
                .OrderByDescending(a => a.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (asistencias, totalRecords);
        }

        public async Task<Asistencia?> GetByIdAsync(int id)
        {
            return await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion.Materia)
                .FirstOrDefaultAsync(a => a.AsistenciaID == id);
        }

        public async Task<IEnumerable<Asistencia>> GetByInscripcionAsync(int inscripcionId)
        {
            return await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion.Materia)
                .Where(a => a.InscripcionID == inscripcionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asistencia>> GetByFechaAsync(DateTime fecha)
        {
            return await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion.Materia)
                .Where(a => a.Fecha.Date == fecha.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Asistencia>> GetByEstudianteAsync(int estudianteId)
        {
            return await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion.Materia)
                .Where(a => a.Inscripcion.EstudianteID == estudianteId)
                .ToListAsync();
        }


        public async Task<bool> ExisteAsync(int inscripcionId, DateTime fecha, int? excludeId = null)
        {
            return await _context.Asistencias.AnyAsync(a =>
                a.InscripcionID == inscripcionId &&
                a.Fecha.Date == fecha.Date &&
                (!excludeId.HasValue || a.AsistenciaID != excludeId.Value));
        }

        public async Task AddAsync(Asistencia asistencia)
        {
            await _context.Asistencias.AddAsync(asistencia);
        }

        public Task UpdateAsync(Asistencia asistencia)
        {
            _context.Asistencias.Update(asistencia);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Asistencia asistencia)
        {
            _context.Asistencias.Remove(asistencia);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
