using Application.DTOs.Calificacion;
using Application.DTOs.Estudiante;
using Application.DTOs.Periodo;
using Application.DTOs.Reportes;
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
    public class ReporteRepository : IReporteRepository
    {
        private readonly ApplicationDbContext _context;

        public ReporteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Estudiante?> GetEstudianteByIdAsync(int estudianteId) =>
            await _context.Estudiantes.FindAsync(estudianteId);

        public async Task<Periodo?> GetPeriodoByIdAsync(int periodoId) =>
            await _context.Periodos.FindAsync(periodoId);

        public async Task<List<Inscripcion>> GetInscripcionesAsync(int estudianteId, int periodoId) =>
            await _context.Inscripciones
                .Include(i => i.Materia).ThenInclude(m => m.Profesor)
                .Include(i => i.Calificaciones)
                .Where(i => i.EstudianteID == estudianteId && i.PeriodoID == periodoId)
                .ToListAsync();

        public async Task<List<Calificacion>> GetCalificacionesPorPeriodoAsync(int periodoId) =>
            await _context.Calificaciones
                .Include(c => c.Inscripcion).ThenInclude(i => i.Materia)
                .Where(c => c.Inscripcion.PeriodoID == periodoId && c.Nota.HasValue)
                .ToListAsync();

        public async Task<List<Asistencia>> GetAsistenciasPorFechaAsync(DateTime fecha) =>
            await _context.Asistencias
                .Include(a => a.Inscripcion).ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion).ThenInclude(i => i.Materia)
                .Where(a => a.Fecha.Date == fecha.Date)
                .ToListAsync();

        public async Task<int> GetTotalEstudiantesAsync() =>
            await _context.Estudiantes.CountAsync();

        public async Task<int> GetEstudiantesActivosAsync() =>
            await _context.Estudiantes.CountAsync(e => e.Activo);

        public async Task<int> GetTotalProfesoresAsync() =>
            await _context.Profesores.CountAsync();

        public async Task<int> GetTotalMateriasAsync() =>
            await _context.Materias.CountAsync();

        public async Task<int> GetTotalInscripcionesAsync() =>
            await _context.Inscripciones.CountAsync();

        public async Task<int> GetPeriodosActivosAsync() =>
            await _context.Periodos.CountAsync(p => p.Activo);

        public async Task<decimal> GetPromedioGeneralCalificacionesAsync() =>
            await _context.Calificaciones.AnyAsync(c => c.Nota.HasValue)
                ? await _context.Calificaciones.Where(c => c.Nota.HasValue).AverageAsync(c => c.Nota ?? 0)
                : 0;

        public async Task<List<Inscripcion>> GetHistorialEstudianteAsync(int estudianteId) =>
            await _context.Inscripciones
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .Include(i => i.Calificaciones)
                .Include(i => i.Asistencias)
                .Where(i => i.EstudianteID == estudianteId)
                .ToListAsync();

        public async Task<List<Estudiante>> GetEstudiantesConCalificacionesPorPeriodoAsync(int periodoId) =>
            await _context.Estudiantes
                .Include(e => e.Inscripciones.Where(i => i.PeriodoID == periodoId))
                    .ThenInclude(i => i.Calificaciones)
                .Where(e => e.Inscripciones.Any(i => i.PeriodoID == periodoId))
                .ToListAsync();
    }
}

