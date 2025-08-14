using Application.DTOs.Materia;
using Application.DTOs.Paginacion;
using Application.DTOs.Profesor;
using Application.DTOs.Reportes;
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
    public class ProfesorService : IProfesorService
    {
        private readonly ApplicationDbContext _context;

        public ProfesorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<ProfesorReadDto> Data, PaginationMetadata Pagination)> GetProfesoresAsync(
            string? filtro = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Profesores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToLower();
                query = query.Where(p =>
                    (p.Nombre != null && p.Nombre.ToLower().Contains(filtro)) ||
                    (p.Apellido != null && p.Apellido.ToLower().Contains(filtro)) ||
                    (p.Correo != null && p.Correo.ToLower().Contains(filtro))
                );
            }

            var totalRecords = await query.CountAsync();
            var profesores = await query
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return (profesores.Select(MapToReadDto), pagination);
        }

        public async Task<ProfesorReadDto?> GetProfesorByIdAsync(int id)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            return profesor is null ? null : MapToReadDto(profesor);
        }

        public async Task<ProfesorReadDto> CreateProfesorAsync(ProfesorCreateDto profesorDto)
        {
            ValidarDatosProfesor(profesorDto.Nombre, profesorDto.Apellido, profesorDto.Correo, profesorDto.Clave);

            if (await ExisteCorreoAsync(profesorDto.Correo))
                throw new InvalidOperationException("Ya existe un profesor con ese correo electrónico");

            var profesor = new Profesor
            {
                Nombre = profesorDto.Nombre.Trim(),
                Apellido = profesorDto.Apellido.Trim(),
                Correo = profesorDto.Correo.Trim().ToLower(),
                Clave = profesorDto.Clave.Trim(),
                FechaCreacion = DateTime.Now
            };

            _context.Profesores.Add(profesor);
            await _context.SaveChangesAsync();

            return MapToReadDto(profesor);
        }

        public async Task<bool> UpdateProfesorAsync(int id, ProfesorUpdateDto profesorDto)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor == null) return false;

            ValidarDatosProfesorUpdate(profesorDto.Nombre, profesorDto.Apellido, profesorDto.Correo, profesorDto.Clave);

            if (await ExisteCorreoAsync(profesorDto.Correo, id))
                throw new InvalidOperationException("Ya existe otro profesor con ese correo electrónico");

            profesor.Nombre = profesorDto.Nombre.Trim();
            profesor.Apellido = profesorDto.Apellido.Trim();
            profesor.Correo = profesorDto.Correo.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(profesorDto.Clave))
                profesor.Clave = profesorDto.Clave.Trim();

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, string Message, bool SoftDelete)> DeleteProfesorAsync(int id)
        {
            var profesor = await _context.Profesores.FindAsync(id);
            if (profesor == null) return (false, "Profesor no encontrado", false);

            var tieneMaterias = await _context.Materias.AnyAsync(m => m.ProfesorID == id);
            if (tieneMaterias)
                return (false, "No se puede eliminar: el profesor tiene materias asignadas", false);

            _context.Profesores.Remove(profesor);
            await _context.SaveChangesAsync();
            return (true, "Profesor eliminado exitosamente", false);
        }

        public async Task<bool> ExisteCorreoAsync(string correo, int? excludeId = null)
        {
            var query = _context.Profesores.Where(p => p.Correo == correo.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(p => p.ProfesorID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<ProfesorConMateriasDto?> GetProfesorConMateriasAsync(int id)
        {
            var profesor = await _context.Profesores
                .Include(p => p.Materias)
                .FirstOrDefaultAsync(p => p.ProfesorID == id);

            return profesor is null ? null : MapToProfesorConMateriasDto(profesor);
        }

        // -------------------------- Helpers --------------------------

        private ProfesorReadDto MapToReadDto(Profesor p) =>
            new ProfesorReadDto
            {
                ProfesorID = p.ProfesorID,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Correo = p.Correo,
                FechaCreacion = p.FechaCreacion
            };

        private ProfesorConMateriasDto MapToProfesorConMateriasDto(Profesor p) =>
            new ProfesorConMateriasDto
            {
                ProfesorID = p.ProfesorID,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Correo = p.Correo,
                Materias = p.Materias.Select(m => new MateriaReadDto
                {
                    MateriaID = m.MateriaID,
                    Nombre = m.Nombre,
                    ProfesorID = m.ProfesorID,
                    NombreProfesor = $"{p.Nombre} {p.Apellido}",
                    FechaCreacion = m.FechaCreacion
                }).ToList()
            };

        private void ValidarDatosProfesor(string nombre, string apellido, string correo, string clave)
        {
            ValidarDatosProfesorUpdate(nombre, apellido, correo, clave);

            if (string.IsNullOrWhiteSpace(clave))
                throw new ArgumentException("La clave es requerida y no puede estar vacía");

            if (clave.Length < 6)
                throw new ArgumentException("La clave debe tener al menos 6 caracteres");
        }

        private void ValidarDatosProfesorUpdate(string nombre, string apellido, string correo, string? clave)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es requerido y no puede estar vacío");
            if (string.IsNullOrWhiteSpace(apellido))
                throw new ArgumentException("El apellido es requerido y no puede estar vacío");
            if (string.IsNullOrWhiteSpace(correo))
                throw new ArgumentException("El correo es requerido y no puede estar vacío");
            if (nombre.Length > 100)
                throw new ArgumentException("El nombre no puede exceder 100 caracteres");
            if (apellido.Length > 100)
                throw new ArgumentException("El apellido no puede exceder 100 caracteres");
            if (correo.Length > 100)
                throw new ArgumentException("El correo no puede exceder 100 caracteres");
            if (!Regex.IsMatch(correo, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                throw new ArgumentException("El formato del correo electrónico no es válido");
            if (!string.IsNullOrWhiteSpace(clave))
            {
                if (clave.Length < 6)
                    throw new ArgumentException("La clave debe tener al menos 6 caracteres");
                if (clave.Length > 255)
                    throw new ArgumentException("La clave no puede exceder 255 caracteres");
            }
        }
    }
}
