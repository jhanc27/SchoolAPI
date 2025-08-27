using Application.DTOs.Inscripcion;
using Application.DTOs.Paginacion;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
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
        private readonly IInscripcionRepository _repository;

        public InscripcionService(IInscripcionRepository repository)
        {
            _repository = repository;
        }

        // Obtener inscripciones con paginación y DTO completo
        public async Task<(IEnumerable<InscripcionReadDto> Data, PaginationMetadata Pagination)>
            GetInscripcionesAsync(int? estudianteId = null, int? materiaId = null, int? periodoId = null, int page = 1, int pageSize = 10)
        {
            var (inscripciones, totalRecords) = await _repository.GetInscripcionesAsync(estudianteId, materiaId, periodoId, page, pageSize);

            // Mapear a DTO completo
            var dtos = inscripciones.Select(MapToReadDto).ToList();

            var pagination = new PaginationMetadata(totalRecords, page, pageSize);
            return (dtos, pagination);
        }

        public async Task<InscripcionReadDto?> GetInscripcionByIdAsync(int id)
        {
            var inscripcion = await _repository.GetByIdAsync(id);
            return inscripcion == null ? null : MapToReadDto(inscripcion);
        }

        public async Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByEstudianteAsync(int estudianteId)
        {
            var inscripciones = await _repository.GetByEstudianteAsync(estudianteId);
            return inscripciones.Select(MapToReadDto);
        }

        public async Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByMateriaAsync(int materiaId)
        {
            var inscripciones = await _repository.GetByMateriaAsync(materiaId);
            return inscripciones.Select(MapToReadDto);
        }

        public async Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByPeriodoAsync(int periodoId)
        {
            var inscripciones = await _repository.GetByPeriodoAsync(periodoId);
            return inscripciones.Select(MapToReadDto);
        }

        public async Task<InscripcionReadDto> CreateInscripcionAsync(InscripcionCreateDto dto)
        {
            var inscripcion = new Inscripcion
            {
                EstudianteID = dto.EstudianteID,
                MateriaID = dto.MateriaID,
                PeriodoID = dto.PeriodoID,
                FechaCreacion = DateTime.UtcNow
            };

            await _repository.AddAsync(inscripcion);

            // Las relaciones ya se cargan dentro de AddAsync
            return MapToReadDto(inscripcion);
        }

        public async Task<bool> UpdateInscripcionAsync(int id, InscripcionUpdateDto dto)
        {
            var inscripcion = await _repository.GetByIdAsync(id);
            if (inscripcion == null) return false;

            inscripcion.EstudianteID = dto.EstudianteID;
            inscripcion.MateriaID = dto.MateriaID;
            inscripcion.PeriodoID = dto.PeriodoID;

            await _repository.UpdateAsync(inscripcion);
            return true;
        }

        public async Task<bool> DeleteInscripcionAsync(int id)
        {
            var inscripcion = await _repository.GetByIdAsync(id);
            if (inscripcion == null) return false;

            await _repository.DeleteAsync(inscripcion);
            return true;
        }

        public Task<bool> ExisteInscripcionAsync(int estudianteId, int materiaId, int periodoId, int? excludeId = null)
            => _repository.ExisteInscripcionAsync(estudianteId, materiaId, periodoId, excludeId);

        public Task<bool> ValidarInscripcionAsync(int estudianteId, int materiaId, int periodoId)
            => _repository.ValidarInscripcionAsync(estudianteId, materiaId, periodoId);

        // Mapeo entidad -> DTO completo con relaciones
        private InscripcionReadDto MapToReadDto(Inscripcion i)
        {
            return new InscripcionReadDto
            {
                InscripcionID = i.InscripcionID,
                EstudianteID = i.EstudianteID,
                NombreEstudiante = i.Estudiante != null ? $"{i.Estudiante.Nombre} {i.Estudiante.Apellido}" : string.Empty,
                MatriculaEstudiante = i.Estudiante?.Matricula ?? string.Empty,
                MateriaID = i.MateriaID,
                NombreMateria = i.Materia?.Nombre ?? string.Empty,
                PeriodoID = i.PeriodoID,
                NombrePeriodo = i.Periodo?.NombrePeriodo ?? string.Empty,
                FechaCreacion = i.FechaCreacion
            };
        }
    }
}
