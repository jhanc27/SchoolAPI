using Application.DTOs.Estudiante;
using Application.DTOs.Paginacion;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EstudianteService : IEstudianteService
    {
        private readonly ApplicationDbContext _context;

        public EstudianteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<EstudianteReadDto> Data, PaginationMetadata Pagination)> GetEstudiantesAsync(
            string? filtro = null, int page = 1, int pageSize = 10)
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

            var totalRecords = await query.CountAsync();

            var estudiantes = await query
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new EstudianteReadDto
                {
                    EstudianteID = e.EstudianteID,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Matricula = e.Matricula,
                    FechaCreacion = e.FechaCreacion,
                    Activo = e.Activo
                })
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

            return (estudiantes, pagination);
        }

        public async Task<EstudianteReadDto?> GetEstudianteByIdAsync(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);

            if (estudiante == null) return null;

            return MapToReadDto(estudiante);
        }

        public async Task<EstudianteReadDto> CreateEstudianteAsync(EstudianteCreateDto dto)
        {
            ValidarDatosEstudiante(dto.Nombre, dto.Apellido, dto.Matricula);

            if (await ExisteMatriculaAsync(dto.Matricula))
                throw new InvalidOperationException("Ya existe un estudiante con esa matrícula");

            var estudiante = new Estudiante
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Matricula = dto.Matricula.Trim().ToUpper(),
                FechaCreacion = DateTime.Now,
                Activo = true
            };

            _context.Estudiantes.Add(estudiante);
            await _context.SaveChangesAsync();

            return MapToReadDto(estudiante);
        }

        public async Task<bool> UpdateEstudianteAsync(int id, EstudianteUpdateDto dto)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null) return false;

            ValidarDatosEstudiante(dto.Nombre, dto.Apellido, dto.Matricula);

            if (await ExisteMatriculaAsync(dto.Matricula, id))
                throw new InvalidOperationException("Ya existe otro estudiante con esa matrícula");

            estudiante.Nombre = dto.Nombre.Trim();
            estudiante.Apellido = dto.Apellido.Trim();
            estudiante.Matricula = dto.Matricula.Trim().ToUpper();
            estudiante.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message, bool SoftDelete)> DeleteEstudianteAsync(int id)
        {
            var estudiante = await _context.Estudiantes.FindAsync(id);
            if (estudiante == null) return (false, "Estudiante no encontrado", false);

            var tieneInscripciones = await _context.Inscripciones.AnyAsync(i => i.EstudianteID == id);
            var tieneCalificaciones = await _context.Calificaciones.AnyAsync(c => c.Inscripcion.EstudianteID == id);

            if (tieneInscripciones || tieneCalificaciones)
            {
                estudiante.Activo = false;
                await _context.SaveChangesAsync();
                return (true, "Estudiante desactivado exitosamente (tiene inscripciones/calificaciones)", true);
            }
            else
            {
                _context.Estudiantes.Remove(estudiante);
                await _context.SaveChangesAsync();
                return (true, "Estudiante eliminado exitosamente", false);
            }
        }

        public async Task<bool> ExisteMatriculaAsync(string matricula, int? excludeId = null)
        {
            var query = _context.Estudiantes.Where(e => e.Matricula == matricula.Trim().ToUpper());
            if (excludeId.HasValue)
                query = query.Where(e => e.EstudianteID != excludeId.Value);

            return await query.AnyAsync();
        }

        private void ValidarDatosEstudiante(string nombre, string apellido, string matricula)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es requerido y no puede estar vacío");

            if (string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El apellido es requerido y no puede estar vacío");

            if (string.IsNullOrWhiteSpace(matricula))
                throw new ArgumentException("La matrícula es requerida y no puede estar vacía");

            if (nombre.Length > 100)
                throw new ArgumentException("El nombre no puede exceder 100 caracteres");

            if (apellido.Length > 100)
                throw new ArgumentException("El apellido no puede exceder 100 caracteres");

            if (matricula.Length > 20)
                throw new ArgumentException("La matrícula no puede exceder 20 caracteres");

            if (!Regex.IsMatch(matricula, @"^[A-Za-z0-9\-]+$"))
                throw new ArgumentException("La matrícula solo puede contener letras, números y guiones");
        }

        private EstudianteReadDto MapToReadDto(Estudiante e) =>
            new()
            {
                EstudianteID = e.EstudianteID,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Matricula = e.Matricula,
                FechaCreacion = e.FechaCreacion,
                Activo = e.Activo
            };
    }
}
