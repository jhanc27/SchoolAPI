using Application.DTOs.Asistencia;
using Application.DTOs.Paginacion;
using Application.DTOs.Reportes;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class AsistenciaService : IAsistenciaService
    {
        private readonly ApplicationDbContext _context;

        public AsistenciaService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Nuevo método con paginación
        public async Task<(IEnumerable<AsistenciaReadDto> Data, PaginationMetadata Pagination)> GetAsistenciasAsync(
            DateTime? fecha = null, int? inscripcionId = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .AsQueryable();

            if (fecha.HasValue)
                query = query.Where(a => a.Fecha.Date == fecha.Value.Date);

            if (inscripcionId.HasValue)
                query = query.Where(a => a.InscripcionID == inscripcionId.Value);

            var totalRecords = await query.CountAsync();

            var data = await query
                .OrderBy(a => a.Fecha)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AsistenciaReadDto
                {
                    AsistenciaID = a.AsistenciaID,
                    InscripcionID = a.InscripcionID,
                    NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                    NombreMateria = a.Inscripcion.Materia.Nombre,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio,
                    FechaCreacion = a.FechaCreacion
                })
                .ToListAsync();

            var pagination = new PaginationMetadata
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                HasPrevious = page > 1,
                HasNext = page < Math.Ceiling((double)totalRecords / pageSize)
            };

            return (data, pagination);
        }

        public async Task<IEnumerable<AsistenciaReadDto>> GetAsistenciasByInscripcionAsync(int inscripcionId)
        {
            return await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .Where(a => a.InscripcionID == inscripcionId)
                .OrderBy(a => a.Fecha)
                .Select(a => new AsistenciaReadDto
                {
                    AsistenciaID = a.AsistenciaID,
                    InscripcionID = a.InscripcionID,
                    NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                    NombreMateria = a.Inscripcion.Materia.Nombre,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio,
                    FechaCreacion = a.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AsistenciaReadDto>> GetAsistenciasByFechaAsync(DateTime fecha)
        {
            return await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .Where(a => a.Fecha.Date == fecha.Date)
                .Select(a => new AsistenciaReadDto
                {
                    AsistenciaID = a.AsistenciaID,
                    InscripcionID = a.InscripcionID,
                    NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                    NombreMateria = a.Inscripcion.Materia.Nombre,
                    Fecha = a.Fecha,
                    Asistio = a.Asistio,
                    FechaCreacion = a.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<AsistenciaReadDto?> GetAsistenciaByIdAsync(int id)
        {
            var a = await _context.Asistencias
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion)
                    .ThenInclude(i => i.Materia)
                .FirstOrDefaultAsync(a => a.AsistenciaID == id);

            if (a == null) return null;

            return new AsistenciaReadDto
            {
                AsistenciaID = a.AsistenciaID,
                InscripcionID = a.InscripcionID,
                NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                NombreMateria = a.Inscripcion.Materia.Nombre,
                Fecha = a.Fecha,
                Asistio = a.Asistio,
                FechaCreacion = a.FechaCreacion
            };
        }

        public async Task<AsistenciaReadDto> CreateAsistenciaAsync(AsistenciaCreateDto dto)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(dto.InscripcionID);
            if (inscripcion == null)
                throw new InvalidOperationException($"No existe una inscripción con ID {dto.InscripcionID}");

            if (await ExisteAsistenciaAsync(dto.InscripcionID, dto.Fecha))
                throw new InvalidOperationException("Ya existe un registro de asistencia para esa fecha e inscripción");

            var a = new Asistencia
            {
                InscripcionID = dto.InscripcionID,
                Fecha = dto.Fecha,
                Asistio = dto.Asistio,
                FechaCreacion = DateTime.Now
            };

            _context.Asistencias.Add(a);
            await _context.SaveChangesAsync();

            return await GetAsistenciaByIdAsync(a.AsistenciaID)
                ?? throw new InvalidOperationException("Error al recuperar la asistencia creada");
        }

        public async Task<bool> UpdateAsistenciaAsync(int id, AsistenciaUpdateDto dto)
        {
            var a = await _context.Asistencias.FindAsync(id);
            if (a == null) return false;

            if (await ExisteAsistenciaAsync(a.InscripcionID, dto.Fecha, id))
                throw new InvalidOperationException("Ya existe otro registro de asistencia para esa fecha");

            a.Fecha = dto.Fecha;
            a.Asistio = dto.Asistio;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsistenciaAsync(int id)
        {
            var a = await _context.Asistencias.FindAsync(id);
            if (a == null) return false;

            _context.Asistencias.Remove(a);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsistenciaAsync(int inscripcionId, DateTime fecha, int? excludeId = null)
        {
            var q = _context.Asistencias
                .Where(a => a.InscripcionID == inscripcionId && a.Fecha.Date == fecha.Date);

            if (excludeId.HasValue)
                q = q.Where(a => a.AsistenciaID != excludeId.Value);

            return await q.AnyAsync();
        }

        public async Task<ReporteAsistenciaDto> GetReporteAsistenciaAsync(int estudianteId, int materiaId)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .FirstOrDefaultAsync(i => i.EstudianteID == estudianteId && i.MateriaID == materiaId);

            if (inscripcion == null)
                throw new InvalidOperationException("No se encontró la inscripción especificada");

            var asistencias = await _context.Asistencias
                .Where(a => a.InscripcionID == inscripcion.InscripcionID)
                .ToListAsync();

            var totalClases = asistencias.Count;
            var clasesAsistidas = asistencias.Count(a => a.Asistio);
            var porcentaje = totalClases > 0 ? Math.Round((clasesAsistidas * 100.0m / totalClases), 2) : 0;

            return new ReporteAsistenciaDto
            {
                EstudianteID = estudianteId,
                NombreEstudiante = $"{inscripcion.Estudiante.Nombre} {inscripcion.Estudiante.Apellido}",
                NombreMateria = inscripcion.Materia.Nombre,
                TotalClases = totalClases,
                ClasesAsistidas = clasesAsistidas,
                PorcentajeAsistencia = porcentaje
            };
        }
    }
}
