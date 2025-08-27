using Application.DTOs.Estudiante;
using Application.DTOs.Paginacion;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
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
        private readonly IEstudianteRepository _repository;

        public EstudianteService(IEstudianteRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<EstudianteReadDto> Data, PaginationMetadata Pagination)> GetEstudiantesAsync(
            string? filtro = null, int page = 1, int pageSize = 10)
        {
            var estudiantes = await _repository.GetAllAsync(filtro, page, pageSize);
            var totalRecords = await _repository.CountAsync(filtro);
            var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            var pagination = new PaginationMetadata(page, pageSize, totalRecords);

            var data = estudiantes.Select(MapToReadDto);

            return (data, pagination);
        }

        public async Task<EstudianteReadDto?> GetEstudianteByIdAsync(int id)
        {
            var estudiante = await _repository.GetByIdAsync(id);
            return estudiante is null ? null : MapToReadDto(estudiante);
        }

        public async Task<EstudianteReadDto> CreateEstudianteAsync(EstudianteCreateDto dto)
        {
            ValidarDatosEstudiante(dto.Nombre, dto.Apellido, dto.Matricula);

            if (await _repository.ExisteMatriculaAsync(dto.Matricula))
                throw new InvalidOperationException("Ya existe un estudiante con esa matrícula");

            var estudiante = new Estudiante
            {
                Nombre = dto.Nombre.Trim(),
                Apellido = dto.Apellido.Trim(),
                Matricula = dto.Matricula.Trim().ToUpper(),
                FechaCreacion = DateTime.Now,
                Activo = true
            };

            await _repository.AddAsync(estudiante);
            return MapToReadDto(estudiante);
        }

        public async Task<bool> UpdateEstudianteAsync(int id, EstudianteUpdateDto dto)
        {
            var estudiante = await _repository.GetByIdAsync(id);
            if (estudiante == null) return false;

            ValidarDatosEstudiante(dto.Nombre, dto.Apellido, dto.Matricula);

            if (await _repository.ExisteMatriculaAsync(dto.Matricula, id))
                throw new InvalidOperationException("Ya existe otro estudiante con esa matrícula");

            estudiante.Nombre = dto.Nombre.Trim();
            estudiante.Apellido = dto.Apellido.Trim();
            estudiante.Matricula = dto.Matricula.Trim().ToUpper();
            estudiante.Activo = dto.Activo;

            await _repository.UpdateAsync(estudiante);
            return true;
        }

        public async Task<(bool Success, string Message, bool SoftDelete)> DeleteEstudianteAsync(int id)
        {
            var estudiante = await _repository.GetByIdAsync(id);
            if (estudiante == null) return (false, "Estudiante no encontrado", false);

            var tieneInscripciones = await _repository.TieneInscripcionesAsync(id);
            var tieneCalificaciones = await _repository.TieneCalificacionesAsync(id);

            if (tieneInscripciones || tieneCalificaciones)
            {
                estudiante.Activo = false;
                await _repository.UpdateAsync(estudiante);
                return (true, "Estudiante desactivado (tiene inscripciones/calificaciones)", true);
            }
            else
            {
                await _repository.DeleteAsync(estudiante);
                return (true, "Estudiante eliminado exitosamente", false);
            }
        }

        public async Task<bool> ExisteMatriculaAsync(string matricula, int? excludeId = null)
        {
            return await _repository.ExisteMatriculaAsync(matricula, excludeId);
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

            if (!System.Text.RegularExpressions.Regex.IsMatch(matricula, @"^[A-Za-z0-9\-]+$"))
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
