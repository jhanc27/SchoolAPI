using Application.DTOs.Materia;
using Application.DTOs.Paginacion;
using Application.DTOs.Profesor;
using Application.DTOs.Reportes;
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
    public class ProfesorService : IProfesorService
    {
        private readonly IProfesorRepository _profesorRepository;

        public ProfesorService(IProfesorRepository profesorRepository)
        {
            _profesorRepository = profesorRepository;
        }

        public async Task<(IEnumerable<ProfesorReadDto> Data, PaginationMetadata Pagination)>
            GetProfesoresAsync(string? filtro = null, int page = 1, int pageSize = 10)
        {
            var profesores = await _profesorRepository.GetAllAsync(filtro, page, pageSize);
            var totalRecords = await _profesorRepository.CountAsync(filtro);

            var pagination = new PaginationMetadata(totalRecords, page, pageSize);

            var data = profesores.Select(p => new ProfesorReadDto
            {
                ProfesorID = p.ProfesorID,
                Nombre = p.Nombre,
                Apellido = p.Apellido,
                Correo = p.Correo,
            });

            return (data, pagination);
        }

        public async Task<ProfesorReadDto?> GetProfesorByIdAsync(int id)
        {
            var profesor = await _profesorRepository.GetByIdAsync(id);
            if (profesor == null) return null;

            return new ProfesorReadDto
            {
                ProfesorID = profesor.ProfesorID,
                Nombre = profesor.Nombre,
                Apellido = profesor.Apellido,
                Correo = profesor.Correo,
            };
        }

        public async Task<ProfesorReadDto> CreateProfesorAsync(ProfesorCreateDto profesorDto)
        {
            var existeCorreo = await _profesorRepository.ExisteCorreoAsync(profesorDto.Correo);
            if (existeCorreo)
                throw new InvalidOperationException("Ya existe un profesor con ese correo.");

            var profesor = new Profesor
            {
                Nombre = profesorDto.Nombre,
                Apellido = profesorDto.Apellido,
                Correo = profesorDto.Correo.Trim().ToLower(),
                Clave = profesorDto.Clave, // ojo: idealmente encriptar
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };

            await _profesorRepository.AddAsync(profesor);

            return new ProfesorReadDto
            {
                ProfesorID = profesor.ProfesorID,
                Nombre = profesor.Nombre,
                Apellido = profesor.Apellido,
                Correo = profesor.Correo,
            };
        }

        public async Task<bool> UpdateProfesorAsync(int id, ProfesorUpdateDto profesorDto)
        {
            var profesor = await _profesorRepository.GetByIdAsync(id);
            if (profesor == null) return false;

            var existeCorreo = await _profesorRepository.ExisteCorreoAsync(profesorDto.Correo, id);
            if (existeCorreo)
                throw new InvalidOperationException("Ya existe otro profesor con ese correo.");

            profesor.Nombre = profesorDto.Nombre;
            profesor.Apellido = profesorDto.Apellido;
            profesor.Correo = profesorDto.Correo.Trim().ToLower();

            await _profesorRepository.UpdateAsync(profesor);
            return true;
        }

        public async Task<(bool Success, string Message, bool SoftDelete)> DeleteProfesorAsync(int id)
        {
            var profesor = await _profesorRepository.GetByIdAsync(id);
            if (profesor == null)
                return (false, "Profesor no encontrado", false);

            if (profesor.Materias.Any()) // si tiene materias, hacemos soft delete
            {
                profesor.Activo = false;
                await _profesorRepository.UpdateAsync(profesor);
                return (true, "Profesor desactivado (soft delete)", true);
            }

            await _profesorRepository.DeleteAsync(profesor);
            return (true, "Profesor eliminado", false);
        }

        public async Task<bool> ExisteCorreoAsync(string correo, int? excludeId = null)
        {
            return await _profesorRepository.ExisteCorreoAsync(correo, excludeId);
        }

        public async Task<ProfesorConMateriasDto?> GetProfesorConMateriasAsync(int id)
        {
            var profesor = await _profesorRepository.GetProfesorConMateriasAsync(id);
            if (profesor == null) return null;

            return new ProfesorConMateriasDto
            {
                ProfesorID = profesor.ProfesorID,
                Nombre = profesor.Nombre,
                Apellido = profesor.Apellido,
                Correo = profesor.Correo,
                Materias = profesor.Materias.Select(m => new MateriaReadDto
                {
                    MateriaID = m.MateriaID,
                    Nombre = m.Nombre
                }).ToList()
            };
        }
    }
}
