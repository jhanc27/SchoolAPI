using Application.DTOs.Calificacion;
using Application.DTOs.Estudiante;
using Application.DTOs.Periodo;
using Application.DTOs.Reportes;
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
    public class ReporteService : IReporteService
    {
        private readonly ApplicationDbContext _context;

        public ReporteService(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<BoletinEstudianteDto?> GetBoletinEstudianteAsync(int estudianteId, int periodoId)
        {
            var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
            if (estudiante == null) return null;

            var periodo = await _context.Periodos.FindAsync(periodoId);
            if (periodo == null) return null;

            var inscripciones = await _context.Inscripciones
                .Include(i => i.Materia).ThenInclude(m => m.Profesor)
                .Include(i => i.Calificaciones)
                .Where(i => i.EstudianteID == estudianteId && i.PeriodoID == periodoId)
                .ToListAsync();

            if (!inscripciones.Any()) return null;

            var materias = inscripciones.Select(i => new MateriaConCalificacionesDto
            {
                Materia = i.Materia.Nombre,
                Profesor = $"{i.Materia.Profesor.Nombre} {i.Materia.Profesor.Apellido}",
                Calificaciones = i.Calificaciones.Select(MapToCalificacionReadDto)
                                               .OrderBy(c => c.FechaCreacion)
                                               .ToList(),
                PromedioMateria = i.Calificaciones.Any() ? Math.Round(i.Calificaciones.Average(c => c.Nota ?? 0), 2) : 0m,
                TotalCalificaciones = i.Calificaciones.Count()
            }).ToList();

            return new BoletinEstudianteDto
            {
                Estudiante = MapToEstudianteReadDto(estudiante),
                Periodo = MapToPeriodoReadDto(periodo),
                Materias = materias,
                PromedioGeneral = materias.Any() ? Math.Round(materias.Average(m => m.PromedioMateria), 2) : 0m
            };
        }

        public async Task<ResumenCursoDto?> GetResumenCursoAsync(int periodoId)
        {
            var periodo = await _context.Periodos.FindAsync(periodoId);
            if (periodo == null) return null;

            var resumenMateriasRaw = await _context.Calificaciones
                .Include(c => c.Inscripcion).ThenInclude(i => i.Materia)
                .Where(c => c.Inscripcion.PeriodoID == periodoId && c.Nota.HasValue)
                .GroupBy(c => c.Inscripcion.Materia.Nombre)
                .Select(g => new
                {
                    Materia = g.Key,
                    TotalEstudiantes = g.Select(c => c.Inscripcion.EstudianteID).Distinct().Count(),
                    TotalCalificaciones = g.Count(),
                    PromedioNota = g.Average(c => (double)c.Nota!.Value),
                    NotaMaxima = g.Max(c => c.Nota!.Value),
                    NotaMinima = g.Min(c => c.Nota!.Value),
                    LiteralesA = g.Count(c => c.Literal == LiteralCalificacion.A),
                    LiteralesB = g.Count(c => c.Literal == LiteralCalificacion.B),
                    LiteralesC = g.Count(c => c.Literal == LiteralCalificacion.C),
                    LiteralesF = g.Count(c => c.Literal == LiteralCalificacion.F),
                    TotalAprobados = g.Count(c => c.Literal != LiteralCalificacion.F)
                })
                .OrderBy(m => m.Materia)
                .ToListAsync();

            if (!resumenMateriasRaw.Any()) return null;

            var resumenMaterias = resumenMateriasRaw.Select(m => new ResumenMateriaDto
            {
                Materia = m.Materia,
                TotalEstudiantes = m.TotalEstudiantes,
                TotalCalificaciones = m.TotalCalificaciones,
                PromedioNota = Math.Round((decimal)m.PromedioNota, 2),
                NotaMaxima = m.NotaMaxima,
                NotaMinima = m.NotaMinima,
                LiteralesA = m.LiteralesA,
                LiteralesB = m.LiteralesB,
                LiteralesC = m.LiteralesC,
                LiteralesF = m.LiteralesF,
                PorcentajeAprobados = m.TotalCalificaciones > 0
                    ? Math.Round((decimal)(m.TotalAprobados * 100.0 / m.TotalCalificaciones), 2)
                    : 0m
            }).ToList();

            return new ResumenCursoDto
            {
                Periodo = MapToPeriodoReadDto(periodo),
                FechaGeneracion = DateTime.Now,
                ResumenPorMateria = resumenMaterias,
                TotalesGenerales = new TotalesGeneralesDto
                {
                    TotalMaterias = resumenMaterias.Count,
                    TotalEstudiantes = resumenMaterias.Sum(r => r.TotalEstudiantes),
                    PromedioGeneral = resumenMaterias.Any()
                        ? Math.Round(resumenMaterias.Average(r => r.PromedioNota), 2)
                        : 0m
                }
            };
        }


        public async Task<ReporteAsistenciaDiariaDto?> GetAsistenciaDiariaAsync(DateTime fecha)
        {
            var raw = await _context.Asistencias
                .Include(a => a.Inscripcion).ThenInclude(i => i.Estudiante)
                .Include(a => a.Inscripcion).ThenInclude(i => i.Materia)
                .Where(a => a.Fecha.Date == fecha.Date)
                .GroupBy(a => a.Inscripcion.Materia.Nombre)
                .Select(g => new
                {
                    Materia = g.Key,
                    TotalEstudiantes = g.Count(),
                    Asistieron = g.Count(a => a.Asistio),
                    Faltaron = g.Count(a => !a.Asistio),
                    Detalles = g.Select(a => new
                    {
                        Estudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                        Matricula = a.Inscripcion.Estudiante.Matricula,
                        Asistio = a.Asistio
                    }).OrderBy(d => d.Estudiante)
                })
                .OrderBy(r => r.Materia)
                .ToListAsync();

            if (!raw.Any()) return null;

            var reporte = raw.Select(r => new AsistenciaPorMateriaDto
            {
                Materia = r.Materia,
                TotalEstudiantes = r.TotalEstudiantes,
                Asistieron = r.Asistieron,
                Faltaron = r.Faltaron,
                PorcentajeAsistencia = r.TotalEstudiantes > 0
                    ? Math.Round((decimal)(r.Asistieron * 100.0 / r.TotalEstudiantes), 2)
                    : 0m,
                Detalles = r.Detalles.Select(d => new DetalleAsistenciaDto
                {
                    Estudiante = d.Estudiante,
                    Matricula = d.Matricula,
                    Asistio = d.Asistio ? "Sí" : "No"
                }).ToList()
            }).ToList();

            return new ReporteAsistenciaDiariaDto
            {
                Fecha = fecha.ToString("yyyy-MM-dd"),
                FechaGeneracion = DateTime.Now,
                ReportePorMateria = reporte,
                ResumenGeneral = new ResumenAsistenciaGeneralDto
                {
                    TotalMaterias = reporte.Count,
                    TotalEstudiantes = reporte.Sum(r => r.TotalEstudiantes),
                    TotalAsistieron = reporte.Sum(r => r.Asistieron),
                    TotalFaltaron = reporte.Sum(r => r.Faltaron),
                    PorcentajeAsistenciaGeneral = reporte.Sum(r => r.TotalEstudiantes) > 0
                        ? Math.Round((decimal)(reporte.Sum(r => r.Asistieron) * 100.0 / reporte.Sum(r => r.TotalEstudiantes)), 2)
                        : 0m
                }
            };
        }

        public async Task<RendimientoEstudianteDto?> GetRendimientoEstudianteAsync(int estudianteId)
        {
            var estudiante = await _context.Estudiantes.FindAsync(estudianteId);
            if (estudiante == null) return null;

            var historial = await _context.Inscripciones
                .Include(i => i.Materia)
                .Include(i => i.Periodo)
                .Include(i => i.Calificaciones)
                .Include(i => i.Asistencias)
                .Where(i => i.EstudianteID == estudianteId)
                .Select(i => new HistorialAcademicoDto
                {
                    Periodo = i.Periodo.NombrePeriodo,
                    Materia = i.Materia.Nombre,
                    PromedioNotas = i.Calificaciones.Any()
                        ? Math.Round((decimal)i.Calificaciones.Average(c => c.Nota ?? 0), 2)
                        : 0m,
                    TotalCalificaciones = i.Calificaciones.Count(),
                    PorcentajeAsistencia = i.Asistencias.Any()
                        ? Math.Round((decimal)(i.Asistencias.Count(a => a.Asistio) * 100.0 / i.Asistencias.Count()), 2)
                        : 0m,
                    TotalClases = i.Asistencias.Count()
                })
                .OrderBy(r => r.Periodo)
                .ThenBy(r => r.Materia)
                .ToListAsync();

            return new RendimientoEstudianteDto
            {
                Estudiante = MapToEstudianteReadDto(estudiante),
                FechaGeneracion = DateTime.Now,
                HistorialAcademico = historial,
                ResumenGeneral = historial.Any() ? new ResumenRendimientoDto
                {
                    PromedioGeneral = Math.Round(historial.Average(r => r.PromedioNotas), 2),
                    AsistenciaPromedio = Math.Round(historial.Average(r => r.PorcentajeAsistencia), 2),
                    TotalMaterias = historial.Count
                } : null
            };
        }

        public async Task<EstadisticasGeneralesDto> GetEstadisticasGeneralesAsync()
        {
            var totalEstudiantes = await _context.Estudiantes.CountAsync();
            var estudiantesActivos = await _context.Estudiantes.CountAsync(e => e.Activo);
            var totalProfesores = await _context.Profesores.CountAsync();
            var totalMaterias = await _context.Materias.CountAsync();
            var totalInscripciones = await _context.Inscripciones.CountAsync();
            var periodosActivos = await _context.Periodos.CountAsync(p => p.Activo);

            var promedioGeneral = await _context.Calificaciones.AnyAsync(c => c.Nota.HasValue)
                ? await _context.Calificaciones.Where(c => c.Nota.HasValue).AverageAsync(c => c.Nota ?? 0)
                : 0;

            var porcentajeAsistencia = await _context.Asistencias.AnyAsync()
                ? await _context.Asistencias.AverageAsync(a => a.Asistio ? 100.0 : 0.0)
                : 0;

            return new EstadisticasGeneralesDto
            {
                TotalEstudiantes = totalEstudiantes,
                EstudiantesActivos = estudiantesActivos,
                TotalProfesores = totalProfesores,
                TotalMaterias = totalMaterias,
                TotalInscripciones = totalInscripciones,
                PeriodosActivos = periodosActivos,
                PromedioGeneralSistema = Math.Round((decimal)promedioGeneral, 2),
                PorcentajeAsistenciaGeneral = Math.Round((decimal)porcentajeAsistencia, 2)
            };
        }

        public async Task<IEnumerable<EstudianteConCalificacionesDto>> GetEstudiantesConCalificacionesAsync(int periodoId)
        {
            return await _context.Estudiantes
                .Include(e => e.Inscripciones.Where(i => i.PeriodoID == periodoId))
                    .ThenInclude(i => i.Calificaciones)
                .Where(e => e.Inscripciones.Any(i => i.PeriodoID == periodoId))
                .Select(e => new EstudianteConCalificacionesDto
                {
                    EstudianteID = e.EstudianteID,
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Matricula = e.Matricula,
                    Calificaciones = e.Inscripciones
                        .Where(i => i.PeriodoID == periodoId)
                        .SelectMany(i => i.Calificaciones)
                        .Select(MapToCalificacionReadDto)
                        .ToList()
                })
                .OrderBy(e => e.Apellido)
                .ThenBy(e => e.Nombre)
                .ToListAsync();
        }


        private EstudianteReadDto MapToEstudianteReadDto(Estudiante e) =>
            new EstudianteReadDto
            {
                EstudianteID = e.EstudianteID,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Matricula = e.Matricula,
                FechaCreacion = e.FechaCreacion,
                Activo = e.Activo
            };

        private PeriodoReadDto MapToPeriodoReadDto(Periodo p) =>
            new PeriodoReadDto
            {
                PeriodoID = p.PeriodoID,
                NombrePeriodo = p.NombrePeriodo,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            };

        private CalificacionReadDto MapToCalificacionReadDto(Calificacion c) =>
            new CalificacionReadDto
            {
                CalificacionID = c.CalificacionID,
                InscripcionID = c.InscripcionID,
                Nota = c.Nota,
                Literal = c.Literal,
                FechaCreacion = c.FechaCreacion
            };

    }
}



