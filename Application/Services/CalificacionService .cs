using Application.DTOs.Calificacion;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
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
        private readonly ApplicationDbContext _context;

        public CalificacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CalificacionReadDto>> GetCalificacionesByInscripcionAsync(int inscripcionId)
        {
            return await _context.Calificaciones
                .Include(c => c.Inscripcion).ThenInclude(i => i.Estudiante)
                .Include(c => c.Inscripcion).ThenInclude(i => i.Materia)
                .Where(c => c.InscripcionID == inscripcionId)
                .OrderBy(c => c.FechaCreacion)
                .Select(c => new CalificacionReadDto
                {
                    CalificacionID = c.CalificacionID,
                    InscripcionID = c.InscripcionID,
                    NombreEstudiante = $"{c.Inscripcion.Estudiante.Nombre} {c.Inscripcion.Estudiante.Apellido}",
                    NombreMateria = c.Inscripcion.Materia.Nombre,
                    Literal = c.Literal,
                    Nota = c.Nota,
                    FechaCreacion = c.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CalificacionReadDto>> GetCalificacionesByEstudianteAsync(int estudianteId)
        {
            return await _context.Calificaciones
                .Include(c => c.Inscripcion).ThenInclude(i => i.Estudiante)
                .Include(c => c.Inscripcion).ThenInclude(i => i.Materia)
                .Where(c => c.Inscripcion.EstudianteID == estudianteId)
                .OrderBy(c => c.FechaCreacion)
                .Select(c => new CalificacionReadDto
                {
                    CalificacionID = c.CalificacionID,
                    InscripcionID = c.InscripcionID,
                    NombreEstudiante = $"{c.Inscripcion.Estudiante.Nombre} {c.Inscripcion.Estudiante.Apellido}",
                    NombreMateria = c.Inscripcion.Materia.Nombre,
                    Literal = c.Literal,
                    Nota = c.Nota,
                    FechaCreacion = c.FechaCreacion
                })
                .ToListAsync();
        }

        public async Task<CalificacionReadDto?> GetCalificacionByIdAsync(int id)
        {
            var c = await _context.Calificaciones
                .Include(c => c.Inscripcion).ThenInclude(i => i.Estudiante)
                .Include(c => c.Inscripcion).ThenInclude(i => i.Materia)
                .FirstOrDefaultAsync(c => c.CalificacionID == id);

            if (c == null) return null;

            return new CalificacionReadDto
            {
                CalificacionID = c.CalificacionID,
                InscripcionID = c.InscripcionID,
                NombreEstudiante = $"{c.Inscripcion.Estudiante.Nombre} {c.Inscripcion.Estudiante.Apellido}",
                NombreMateria = c.Inscripcion.Materia.Nombre,
                Literal = c.Literal,
                Nota = c.Nota,
                FechaCreacion = c.FechaCreacion
            };
        }

        public async Task<CalificacionReadDto> CreateCalificacionAsync(CalificacionCreateDto dto)
        {
            var inscripcion = await _context.Inscripciones
                .Include(i => i.Estudiante)
                .FirstOrDefaultAsync(i => i.InscripcionID == dto.InscripcionID);

            if (inscripcion == null)
                throw new InvalidOperationException($"No existe una inscripción con ID {dto.InscripcionID}");

            if (!inscripcion.Estudiante.Activo)
                throw new InvalidOperationException("No se puede calificar a un estudiante inactivo");

            var calificacion = new Calificacion
            {
                InscripcionID = dto.InscripcionID,
                Literal = dto.Literal, // LiteralCalificacion no nullable
                Nota = dto.Nota,
                FechaCreacion = DateTime.Now
            };

            AsignarNotaLiteral(ref calificacion);

            _context.Calificaciones.Add(calificacion);
            await _context.SaveChangesAsync();

            return await GetCalificacionByIdAsync(calificacion.CalificacionID)
                   ?? throw new InvalidOperationException("Error al recuperar la calificación creada");
        }

        public async Task<bool> UpdateCalificacionAsync(int id, CalificacionUpdateDto dto)
        {
            var c = await _context.Calificaciones.FindAsync(id);
            if (c == null) return false;

            c.Literal = dto.Literal;
            c.Nota = dto.Nota;

            AsignarNotaLiteral(ref c);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteCalificacionAsync(int id)
        {
            var c = await _context.Calificaciones.FindAsync(id);
            if (c == null) return false;

            _context.Calificaciones.Remove(c);
            await _context.SaveChangesAsync();
            return true;
        }

        private void AsignarNotaLiteral(ref Calificacion calificacion)
        {
            if (calificacion.Nota.HasValue && calificacion.Literal == 0) // 0 = valor por defecto del enum
                calificacion.Literal = CalcularLiteralPorNota(calificacion.Nota.Value);

            if (!calificacion.Nota.HasValue && calificacion.Literal != 0)
                calificacion.Nota = CalcularNotaPorLiteral(calificacion.Literal);

            if (calificacion.Nota.HasValue && calificacion.Literal != 0)
            {
                if (!ValidarConsistenciaNotaLiteral(calificacion.Nota.Value, calificacion.Literal))
                    throw new InvalidOperationException("La nota y el literal no son consistentes");
            }
        }

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
                LiteralCalificacion.A => 95,
                LiteralCalificacion.B => 85,
                LiteralCalificacion.C => 75,
                LiteralCalificacion.F => 50, // F es cualquier nota menor de 70
                _ => 0
            };
        }

        public bool ValidarConsistenciaNotaLiteral(decimal nota, LiteralCalificacion literal)
        {
            return literal switch
            {
                LiteralCalificacion.A => nota >= 90 && nota <= 100,
                LiteralCalificacion.B => nota >= 80 && nota < 90,
                LiteralCalificacion.C => nota >= 70 && nota < 80,
                LiteralCalificacion.F => nota >= 0 && nota < 70,
                _ => false
            };
        }
    }
}
