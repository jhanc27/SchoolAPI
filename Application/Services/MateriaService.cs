using Application.DTOs.Materia;
using Application.DTOs.Paginacion;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
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
        private readonly IMateriaRepository _materiaRepository;

        public MateriaService(IMateriaRepository materiaRepository)
        {
            _materiaRepository = materiaRepository;
        }

        public async Task<(IEnumerable<MateriaReadDto> Data, PaginationMetadata Pagination)> GetMateriasAsync(
           string? filtro = null,
           int page = 1,
           int pageSize = 10)
        {
            var (materias, totalRecords) = await _materiaRepository.GetMateriasAsync(filtro, page, pageSize);

            var materiaDtos = materias.Select(m => new MateriaReadDto
            {
                MateriaID = m.MateriaID,
                Nombre = m.Nombre,
                ProfesorID = m.ProfesorID,
                NombreProfesor = $"{m.Profesor.Nombre} {m.Profesor.Apellido}",
                FechaCreacion = m.FechaCreacion
            }).ToList();

            // ✅ Usando el constructor con parámetros
            var pagination = new PaginationMetadata(page, pageSize, totalRecords);

            return (materiaDtos, pagination);
        }


        public async Task<MateriaReadDto?> GetMateriaByIdAsync(int id)
        {
            var materia = await _materiaRepository.GetMateriaByIdAsync(id);
            if (materia == null) return null;

            return new MateriaReadDto
            {
                MateriaID = materia.MateriaID,
                Nombre = materia.Nombre,
                ProfesorID = materia.ProfesorID,
                NombreProfesor = $"{materia.Profesor.Nombre} {materia.Profesor.Apellido}",
                FechaCreacion = materia.FechaCreacion
            };
        }

        public async Task<MateriaReadDto> CreateMateriaAsync(MateriaCreateDto materiaDto)
        {
            var materia = new Materia
            {
                Nombre = materiaDto.Nombre,
                ProfesorID = materiaDto.ProfesorID,
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };

            await _materiaRepository.AddAsync(materia);
            await _materiaRepository.SaveChangesAsync();

            // Recargar materia incluyendo profesor
            var materiaConProfesor = await _materiaRepository.GetMateriaByIdAsync(materia.MateriaID);

            return new MateriaReadDto
            {
                MateriaID = materia.MateriaID,
                Nombre = materia.Nombre,
                ProfesorID = materia.ProfesorID,
                NombreProfesor = materiaConProfesor?.Profesor != null
                    ? $"{materiaConProfesor.Profesor.Nombre} {materiaConProfesor.Profesor.Apellido}"
                    : string.Empty,
                FechaCreacion = materia.FechaCreacion
            };
        }

        public async Task<bool> UpdateMateriaAsync(int id, MateriaUpdateDto materiaDto)
        {
            var materia = await _materiaRepository.GetMateriaByIdAsync(id);
            if (materia == null) return false;

            materia.Nombre = materiaDto.Nombre;
            materia.ProfesorID = materiaDto.ProfesorID;

            await _materiaRepository.UpdateAsync(materia);
            await _materiaRepository.SaveChangesAsync();

            return true;
        }

        public async Task<(bool Success, string Message)> DeleteMateriaAsync(int id)
        {
            var materia = await _materiaRepository.GetMateriaByIdAsync(id);
            if (materia == null)
                return (false, "La materia no existe.");

            var tieneInscripciones = await _materiaRepository.HasInscripcionesAsync(id);
            if (tieneInscripciones)
                return (false, "No se puede eliminar la materia porque tiene inscripciones asociadas.");

            await _materiaRepository.DeleteAsync(materia);
            await _materiaRepository.SaveChangesAsync();

            return (true, "Materia eliminada correctamente.");
        }

        public async Task<IEnumerable<MateriaReadDto>> GetMateriasByProfesorAsync(int profesorId)
        {
            var materias = await _materiaRepository.GetMateriasByProfesorAsync(profesorId);

            return materias.Select(m => new MateriaReadDto
            {
                MateriaID = m.MateriaID,
                Nombre = m.Nombre,
                ProfesorID = m.ProfesorID,
                NombreProfesor = $"{m.Profesor.Nombre} {m.Profesor.Apellido}"
            }).ToList();
        }
    }
}
