using Application.DTOs.Asistencia;
using Application.DTOs.Paginacion;
using Application.DTOs.Reportes;
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
    public class AsistenciaService : IAsistenciaService
    {
        private readonly IAsistenciaRepository _repository;

        public AsistenciaService(IAsistenciaRepository repository)
        {
            _repository = repository;
        }

        public async Task<(IEnumerable<AsistenciaReadDto> Data, PaginationMetadata Pagination)> GetAsistenciasAsync(
            DateTime? fecha = null, int? inscripcionId = null, int page = 1, int pageSize = 10)
        {
            var (asistencias, totalRecords) = await _repository.GetAsync(fecha, inscripcionId, page, pageSize);

            var data = asistencias.Select(a => new AsistenciaReadDto
            {
                AsistenciaID = a.AsistenciaID,
                InscripcionID = a.InscripcionID,
                NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                NombreMateria = a.Inscripcion.Materia.Nombre,
                Fecha = a.Fecha,
                Asistio = a.Asistio,
                FechaCreacion = a.FechaCreacion
            }).ToList();

            var pagination = new PaginationMetadata(page, pageSize, totalRecords);

            return (data, pagination);
        }

        public async Task<IEnumerable<AsistenciaReadDto>> GetAsistenciasByInscripcionAsync(int inscripcionId)
        {
            var asistencias = await _repository.GetByInscripcionAsync(inscripcionId);

            return asistencias.Select(a => new AsistenciaReadDto
            {
                AsistenciaID = a.AsistenciaID,
                InscripcionID = a.InscripcionID,
                NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                NombreMateria = a.Inscripcion.Materia.Nombre,
                Fecha = a.Fecha,
                Asistio = a.Asistio,
                FechaCreacion = a.FechaCreacion
            });
        }

        public async Task<IEnumerable<AsistenciaReadDto>> GetAsistenciasByFechaAsync(DateTime fecha)
        {
            var asistencias = await _repository.GetByFechaAsync(fecha);

            return asistencias.Select(a => new AsistenciaReadDto
            {
                AsistenciaID = a.AsistenciaID,
                InscripcionID = a.InscripcionID,
                NombreEstudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                NombreMateria = a.Inscripcion.Materia.Nombre,
                Fecha = a.Fecha,
                Asistio = a.Asistio,
                FechaCreacion = a.FechaCreacion
            });
        }

        public async Task<AsistenciaReadDto?> GetAsistenciaByIdAsync(int id)
        {
            var a = await _repository.GetByIdAsync(id);
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
            if (await _repository.ExisteAsync(dto.InscripcionID, dto.Fecha))
                throw new InvalidOperationException("Ya existe un registro de asistencia para esa fecha e inscripción");

            var a = new Asistencia
            {
                InscripcionID = dto.InscripcionID,
                Fecha = dto.Fecha,
                Asistio = dto.Asistio,
                FechaCreacion = DateTime.Now
            };

            await _repository.AddAsync(a);
            await _repository.SaveChangesAsync();

            var created = await GetAsistenciaByIdAsync(a.AsistenciaID);
            if (created == null)
                throw new InvalidOperationException("Error al recuperar la asistencia creada");

            return created;
        }

        public async Task<bool> UpdateAsistenciaAsync(int id, AsistenciaUpdateDto dto)
        {
            var a = await _repository.GetByIdAsync(id);
            if (a == null) return false;

            if (await _repository.ExisteAsync(a.InscripcionID, dto.Fecha, id))
                throw new InvalidOperationException("Ya existe otro registro de asistencia para esa fecha");

            a.Fecha = dto.Fecha;
            a.Asistio = dto.Asistio;

            await _repository.UpdateAsync(a);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsistenciaAsync(int id)
        {
            var a = await _repository.GetByIdAsync(id);
            if (a == null) return false;

            await _repository.DeleteAsync(a);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExisteAsistenciaAsync(int inscripcionId, DateTime fecha, int? excludeId = null)
        {
            return await _repository.ExisteAsync(inscripcionId, fecha, excludeId);
        }

        public async Task<ReporteAsistenciaDto> GetReporteAsistenciaAsync(int estudianteId, int materiaId)
        {
            // Obtiene todas las asistencias del estudiante
            var asistenciasEstudiante = await _repository.GetByEstudianteAsync(estudianteId);

            // Busca si el estudiante está inscrito en la materia solicitada
            var primeraAsistencia = asistenciasEstudiante
                .FirstOrDefault(a => a.Inscripcion.MateriaID == materiaId);

            if (primeraAsistencia == null)
                throw new InvalidOperationException("No se encontró la inscripción especificada");

            // Obtiene todas las asistencias de esa inscripción (no solo la primera)
            var asistencias = await _repository.GetByInscripcionAsync(primeraAsistencia.InscripcionID);

            var totalClases = asistencias.Count();
            var clasesAsistidas = asistencias.Count(a => a.Asistio);
            var porcentaje = totalClases > 0
                ? Math.Round((clasesAsistidas * 100.0m / totalClases), 2)
                : 0;

            return new ReporteAsistenciaDto
            {
                EstudianteID = estudianteId,
                NombreEstudiante = $"{primeraAsistencia.Inscripcion.Estudiante.Nombre} {primeraAsistencia.Inscripcion.Estudiante.Apellido}",
                NombreMateria = primeraAsistencia.Inscripcion.Materia.Nombre,
                TotalClases = totalClases,
                ClasesAsistidas = clasesAsistidas,
                PorcentajeAsistencia = porcentaje
            };
        }

      
    }
}
