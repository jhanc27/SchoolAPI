using Application.DTOs.Inscripcion;
using Application.DTOs.Paginacion;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class InscripcionService : IInscripcionService
    {
        private readonly ApplicationDbContext _context;

        public InscripcionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<InscripcionReadDto> Data, PaginationMetadata Pagination)> GetInscripcionesAsync(
            int? estudianteId = null, int? materiaId = null, int? periodoId = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .AsQueryable();

            if (estudianteId.HasValue) query = query.Where(i => i.EstudianteID == estudianteId.Value);
            if (materiaId.HasValue) query = query.Where(i => i.MateriaID == materiaId.Value);
            if (periodoId.HasValue) query = query.Where(i => i.PeriodoID == periodoId.Value);

            var totalRecords = await query.CountAsync();

            var inscripciones = await query
                .OrderBy(i => i.FechaCreacion)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => MapToReadDto(i))
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagination = new PaginationMetadata
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                HasPrevious = page > 1,
                HasNext = page < totalPages
            };

            return (inscripciones, pagination);
        }

        public async Task<InscripcionReadDto?> GetInscripcionByIdAsync(int id)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .FirstOrDefaultAsync(i => i.InscripcionID == id);

            return inscripcion == null ? null : MapToReadDto(inscripcion);
        }

        public async Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByEstudianteAsync(int estudianteId) =>
            await GetFilteredInscripciones(i => i.EstudianteID == estudianteId);

        public async Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByMateriaAsync(int materiaId) =>
            await GetFilteredInscripciones(i => i.MateriaID == materiaId);

        public async Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByPeriodoAsync(int periodoId) =>
            await GetFilteredInscripciones(i => i.PeriodoID == periodoId);

        private async Task<IEnumerable<InscripcionReadDto>> GetFilteredInscripciones(
            Expression<Func<Inscripcion, bool>> filter)
        {
            return await _context.Inscripciones
                .Include(i => i.Estudiante)
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .Where(filter)
                .Select(i => MapToReadDto(i))
                .ToListAsync();
        }

        public async Task<InscripcionReadDto> CreateInscripcionAsync(InscripcionCreateDto dto)
        {
            if (!await ValidarInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID))
                throw new InvalidOperationException("Validación de inscripción fallida");

            if (await ExisteInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID))
                throw new InvalidOperationException("El estudiante ya está inscrito en esta materia para este período");

            var inscripcion = new Inscripcion
            {
                EstudianteID = dto.EstudianteID,
                MateriaID = dto.MateriaID,
                PeriodoID = dto.PeriodoID,
                FechaCreacion = DateTime.Now
            };

            _context.Inscripciones.Add(inscripcion);
            await _context.SaveChangesAsync();

            return await GetInscripcionByIdAsync(inscripcion.InscripcionID) ??
                   throw new InvalidOperationException("Error al recuperar la inscripción creada");
        }

        public async Task<bool> UpdateInscripcionAsync(int id, InscripcionUpdateDto dto)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null) return false;

            if (!await ValidarInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID))
                throw new InvalidOperationException("Validación de inscripción fallida");

            if (await ExisteInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID, id))
                throw new InvalidOperationException("Ya existe otra inscripción con esos datos");

            inscripcion.EstudianteID = dto.EstudianteID;
            inscripcion.MateriaID = dto.MateriaID;
            inscripcion.PeriodoID = dto.PeriodoID;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteInscripcionAsync(int id)
        {
            var inscripcion = await _context.Inscripciones.FindAsync(id);
            if (inscripcion == null) return false;

            _context.Inscripciones.Remove(inscripcion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteInscripcionAsync(int estudianteId, int materiaId, int periodoId, int? excludeId = null)
        {
            var query = _context.Inscripciones.Where(i =>
                i.EstudianteID == estudianteId &&
                i.MateriaID == materiaId &&
                i.PeriodoID == periodoId);

            if (excludeId.HasValue)
                query = query.Where(i => i.InscripcionID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ValidarInscripcionAsync(int estudianteId, int materiaId, int periodoId)
        {
            var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
            if (estudiante == null || !estudiante.Activo)
                throw new InvalidOperationException("El estudiante no existe o está inactivo");

            var materia = await _context.Materias.FindAsync(materiaId);
            if (materia == null)
                throw new InvalidOperationException("La materia no existe");

            var periodo = await _context.Periodos.FindAsync(periodoId);
            if (periodo == null || !periodo.Activo)
                throw new InvalidOperationException("El período no existe o está inactivo");

            if (DateTime.Now < periodo.FechaInicio)
                throw new InvalidOperationException("El período de inscripción aún no ha comenzado");

            if (DateTime.Now > periodo.FechaFin)
                throw new InvalidOperationException("El período de inscripción ya ha finalizado");

            return true;
        }

        private static InscripcionReadDto MapToReadDto(Inscripcion i) =>
            new()
            {
                InscripcionID = i.InscripcionID,
                EstudianteID = i.EstudianteID,
                NombreEstudiante = $"{i.Estudiante.Nombre} {i.Estudiante.Apellido}",
                MatriculaEstudiante = i.Estudiante.Matricula,
                MateriaID = i.MateriaID,
                NombreMateria = i.Materia.Nombre,
                PeriodoID = i.PeriodoID,
                NombrePeriodo = i.Periodo.NombrePeriodo,
                FechaCreacion = i.FechaCreacion
            };
    }
}
