using Application.DTOs.Materia;
using Application.DTOs.Paginacion;
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
    public class MateriaService : IMateriaService
    {
        private readonly ApplicationDbContext _context;

        public MateriaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<MateriaReadDto> Data, PaginationMetadata Pagination)> GetMateriasAsync(
            string? filtro = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Materias
                .Include(m => m.Profesor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = $"%{filtro.ToLower()}%";
                query = query.Where(m =>
                    (m.Nombre != null && EF.Functions.Like(m.Nombre.ToLower(), filtro)) ||
                    (m.Profesor != null && m.Profesor.Nombre != null && EF.Functions.Like(m.Profesor.Nombre.ToLower(), filtro)) ||
                    (m.Profesor != null && m.Profesor.Apellido != null && EF.Functions.Like(m.Profesor.Apellido.ToLower(), filtro))
                );
            }

            var totalRecords = await query.CountAsync();

            var materias = await query
                .OrderBy(m => m.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => MapToReadDto(m))
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

            return (materias, pagination);
        }

        public async Task<MateriaReadDto?> GetMateriaByIdAsync(int id)
        {
            var materia = await _context.Materias
                .Include(m => m.Profesor)
                .FirstOrDefaultAsync(m => m.MateriaID == id);

            return materia is null ? null : MapToReadDto(materia);
        }

        public async Task<MateriaReadDto> CreateMateriaAsync(MateriaCreateDto materiaDto)
        {
            ValidarDatosMateria(materiaDto.Nombre, materiaDto.ProfesorID);

            var profesor = await _context.Profesores.FindAsync(materiaDto.ProfesorID)
                ?? throw new InvalidOperationException($"No existe un profesor con ID {materiaDto.ProfesorID}");

            var materia = new Materia
            {
                Nombre = materiaDto.Nombre.Trim(),
                ProfesorID = materiaDto.ProfesorID,
                FechaCreacion = DateTime.Now
            };

            _context.Materias.Add(materia);
            await _context.SaveChangesAsync();

            // Mapear y devolver
            materia.Profesor = profesor;
            return MapToReadDto(materia);
        }

        public async Task<bool> UpdateMateriaAsync(int id, MateriaUpdateDto materiaDto)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia == null) return false;

            ValidarDatosMateria(materiaDto.Nombre, materiaDto.ProfesorID);

            var profesor = await _context.Profesores.FindAsync(materiaDto.ProfesorID)
                ?? throw new InvalidOperationException($"No existe un profesor con ID {materiaDto.ProfesorID}");

            materia.Nombre = materiaDto.Nombre.Trim();
            materia.ProfesorID = materiaDto.ProfesorID;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message)> DeleteMateriaAsync(int id)
        {
            var materia = await _context.Materias.FindAsync(id);
            if (materia == null) return (false, "Materia no encontrada");

            var tieneInscripciones = await _context.Inscripciones.AnyAsync(i => i.MateriaID == id);
            if (tieneInscripciones)
                return (false, "No se puede eliminar: la materia tiene inscripciones");

            _context.Materias.Remove(materia);
            await _context.SaveChangesAsync();
            return (true, "Materia eliminada exitosamente");
        }

        public async Task<IEnumerable<MateriaReadDto>> GetMateriasByProfesorAsync(int profesorId)
        {
            var materias = await _context.Materias
                .Include(m => m.Profesor)
                .Where(m => m.ProfesorID == profesorId)
                .ToListAsync();

            return materias.Select(m => MapToReadDto(m));
        }

        // -------------------------- Helpers --------------------------

        private MateriaReadDto MapToReadDto(Materia m) =>
            new MateriaReadDto
            {
                MateriaID = m.MateriaID,
                Nombre = m.Nombre,
                ProfesorID = m.ProfesorID,
                NombreProfesor = m.Profesor != null
                    ? $"{m.Profesor.Nombre} {m.Profesor.Apellido}"
                    : "Sin profesor",
                FechaCreacion = m.FechaCreacion
            };

        private void ValidarDatosMateria(string nombre, int profesorId)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la materia es requerido y no puede estar vacío");

            if (nombre.Length > 100)
                throw new ArgumentException("El nombre no puede exceder 100 caracteres");

            if (profesorId <= 0)
                throw new ArgumentException("El ID del profesor debe ser mayor a 0");
        }
    }
}
