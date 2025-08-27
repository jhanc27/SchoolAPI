using Application.DTOs.Calificacion;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class CalificacionService : ICalificacionService
    {
        private readonly ICalificacionRepository _repository;

        public CalificacionService(ICalificacionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CalificacionReadDto>> GetCalificacionesByInscripcionAsync(int inscripcionId)
        {
            var calificaciones = await _repository.GetByInscripcionAsync(inscripcionId);

            return calificaciones.Select(c => new CalificacionReadDto
            {
                CalificacionID = c.CalificacionID,
                InscripcionID = c.InscripcionID,
                NombreEstudiante = $"{c.Inscripcion.Estudiante.Nombre} {c.Inscripcion.Estudiante.Apellido}",
                NombreMateria = c.Inscripcion.Materia.Nombre,
                Literal = c.Literal.ToString(),
                Nota = c.Nota,
                FechaCreacion = c.FechaCreacion
            });
        }

        public async Task<IEnumerable<CalificacionReadDto>> GetCalificacionesByEstudianteAsync(int estudianteId)
        {
            var calificaciones = await _repository.GetByEstudianteAsync(estudianteId);

            return calificaciones.Select(c => new CalificacionReadDto
            {
                CalificacionID = c.CalificacionID,
                InscripcionID = c.InscripcionID,
                NombreEstudiante = $"{c.Inscripcion.Estudiante.Nombre} {c.Inscripcion.Estudiante.Apellido}",
                NombreMateria = c.Inscripcion.Materia.Nombre,
                Literal = c.Literal.ToString(),
                Nota = c.Nota,
                FechaCreacion = c.FechaCreacion
            });
        }

        public async Task<CalificacionReadDto?> GetCalificacionByIdAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return null;

            return new CalificacionReadDto
            {
                CalificacionID = c.CalificacionID,
                InscripcionID = c.InscripcionID,
                NombreEstudiante = $"{c.Inscripcion.Estudiante.Nombre} {c.Inscripcion.Estudiante.Apellido}",
                NombreMateria = c.Inscripcion.Materia.Nombre,
                Literal = c.Literal.ToString(),
                Nota = c.Nota,
                FechaCreacion = c.FechaCreacion
            };
        }

        public async Task<CalificacionReadDto> CreateCalificacionAsync(CalificacionCreateDto dto)
        {
            var calificacion = new Calificacion
            {
                InscripcionID = dto.InscripcionID,
                Nota = dto.Nota,
                Literal = CalcularLiteralPorNota(dto.Nota), // 🔹 se asigna aquí
                FechaCreacion = DateTime.Now
            };

            await _repository.AddAsync(calificacion);

            return await GetCalificacionByIdAsync(calificacion.CalificacionID)
                   ?? throw new InvalidOperationException("Error al recuperar la calificación creada");
        }

        public async Task<bool> UpdateCalificacionAsync(int id, CalificacionUpdateDto dto)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return false;

            c.Nota = dto.Nota;
            c.Literal = CalcularLiteralPorNota(dto.Nota); // 🔹 recalcula literal

            await _repository.UpdateAsync(c);
            return true;
        }

        public async Task<bool> DeleteCalificacionAsync(int id)
        {
            var c = await _repository.GetByIdAsync(id);
            if (c == null) return false;

            await _repository.DeleteAsync(c);
            return true;
        }

        // 🔹 Cálculo único de literal por nota
        public LiteralCalificacion CalcularLiteralPorNota(decimal nota)
        {
            return nota switch
            {
                >= 90 => LiteralCalificacion.A,
                >= 80 => LiteralCalificacion.B,
                >= 70 => LiteralCalificacion.C,
                _ => LiteralCalificacion.F
            };
        }

        public decimal CalcularNotaPorLiteral(LiteralCalificacion literal)
        {
            return literal switch
            {
                LiteralCalificacion.A => 90m,
                LiteralCalificacion.B => 80m,
                LiteralCalificacion.C => 70m,
                LiteralCalificacion.F => 0m,
                _ => throw new ArgumentOutOfRangeException(nameof(literal), "Literal no válido")
            };
        }
    }
}
