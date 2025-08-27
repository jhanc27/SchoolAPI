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
    public class InscripcionRepository : IInscripcionRepository
    {
        private readonly ApplicationDbContext _context;

        public InscripcionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Obtiene inscripciones con paginación y relaciones cargadas
        public async Task<(IEnumerable<Inscripcion> Data, int TotalCount)> GetInscripcionesAsync(
            int? estudianteId = null,
            int? materiaId = null,
            int? periodoId = null,
            int page = 1,
            int pageSize = 10)
        {
            var query = _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .AsQueryable();

            if (estudianteId.HasValue)
                query = query.Where(i => i.EstudianteID == estudianteId.Value);

            if (materiaId.HasValue)
                query = query.Where(i => i.MateriaID == materiaId.Value);

            if (periodoId.HasValue)
                query = query.Where(i => i.PeriodoID == periodoId.Value);

            var totalCount = await query.CountAsync();

            var data = await query
                .OrderByDescending(i => i.FechaCreacion)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, totalCount);
        }

        public async Task<Inscripcion?> GetByIdAsync(int id)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .FirstOrDefaultAsync(i => i.InscripcionID == id);
        }

        public async Task<IEnumerable<Inscripcion>> GetByEstudianteAsync(int estudianteId)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .Where(i => i.EstudianteID == estudianteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inscripcion>> GetByMateriaAsync(int materiaId)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .Where(i => i.MateriaID == materiaId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Inscripcion>> GetByPeriodoAsync(int periodoId)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .Where(i => i.PeriodoID == periodoId)
                .ToListAsync();
        }

        public async Task AddAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            // Cargar relaciones para que el DTO tenga todos los campos completos
            await _context.Entry(inscripcion).Reference(i => i.Estudiante).LoadAsync();
            await _context.Entry(inscripcion).Reference(i => i.Materia).LoadAsync();
            await _context.Entry(inscripcion).Reference(i => i.Periodo).LoadAsync();
        }

        public async Task UpdateAsync(Inscripcion inscripcion)
        {
            _context.Entry(inscripcion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            // Asegurar que relaciones estén cargadas después de actualizar
            await _context.Entry(inscripcion).Reference(i => i.Estudiante).LoadAsync();
            await _context.Entry(inscripcion).Reference(i => i.Materia).LoadAsync();
            await _context.Entry(inscripcion).Reference(i => i.Periodo).LoadAsync();
        }

        public async Task DeleteAsync(Inscripcion inscripcion)
        {
            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteInscripcionAsync(int estudianteId, int materiaId, int periodoId, int? excludeId = null)
        {
            var query = _context.Inscripciones
                .Where(i => i.EstudianteID == estudianteId && i.MateriaID == materiaId && i.PeriodoID == periodoId);

            if (excludeId.HasValue)
                query = query.Where(i => i.InscripcionID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ValidarInscripcionAsync(int estudianteId, int materiaId, int periodoId)
        {
            return await _context.Estudiantes.AnyAsync(e => e.EstudianteID == estudianteId)
                && await _context.Materias.AnyAsync(m => m.MateriaID == materiaId)
                && await _context.Periodos.AnyAsync(p => p.PeriodoID == periodoId);
        }
    }
}
