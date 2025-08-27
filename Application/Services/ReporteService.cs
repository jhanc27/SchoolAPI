using Application.DTOs.Calificacion;
using Application.DTOs.Estudiante;
using Application.DTOs.Periodo;
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
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _reporteRepository;

        public ReporteService(IReporteRepository reporteRepository)
        {
            _reporteRepository = reporteRepository;
        }

        public async Task<BoletinEstudianteDto?> GetBoletinEstudianteAsync(int estudianteId, int periodoId)
        {
            var estudiante = await _reporteRepository.GetEstudianteByIdAsync(estudianteId);
            if (estudiante == null) return null;

            var periodo = await _reporteRepository.GetPeriodoByIdAsync(periodoId);
            if (periodo == null) return null;

            var inscripciones = await _reporteRepository.GetInscripcionesAsync(estudianteId, periodoId);
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
            var periodo = await _reporteRepository.GetPeriodoByIdAsync(periodoId);
            if (periodo == null) return null;

            var calificaciones = await _reporteRepository.GetCalificacionesPorPeriodoAsync(periodoId);
            if (!calificaciones.Any()) return null;

            var resumenMaterias = calificaciones
                .GroupBy(c => c.Inscripcion.Materia.Nombre)
                .Select(g => new ResumenMateriaDto
                {
                    Materia = g.Key,
                    TotalEstudiantes = g.Select(c => c.Inscripcion.EstudianteID).Distinct().Count(),
                    TotalCalificaciones = g.Count(),
                    PromedioNota = Math.Round((decimal)g.Average(c => c.Nota ?? 0), 2),
                    NotaMaxima = g.Max(c => c.Nota ?? 0),
                    NotaMinima = g.Min(c => c.Nota ?? 0),
                    LiteralesA = g.Count(c => c.Literal == LiteralCalificacion.A),
                    LiteralesB = g.Count(c => c.Literal == LiteralCalificacion.B),
                    LiteralesC = g.Count(c => c.Literal == LiteralCalificacion.C),
                    LiteralesF = g.Count(c => c.Literal == LiteralCalificacion.F),
                    PorcentajeAprobados = g.Count(c => c.Literal != LiteralCalificacion.F) * 100m / g.Count()
                })
                .OrderBy(r => r.Materia)
                .ToList();

            return new ResumenCursoDto
            {
                Periodo = MapToPeriodoReadDto(periodo),
                FechaGeneracion = DateTime.Now,
                ResumenPorMateria = resumenMaterias,
                TotalesGenerales = new TotalesGeneralesDto
                {
                    TotalMaterias = resumenMaterias.Count,
                    TotalEstudiantes = resumenMaterias.Sum(r => r.TotalEstudiantes),
                    PromedioGeneral = resumenMaterias.Any() ? Math.Round(resumenMaterias.Average(r => r.PromedioNota), 2) : 0m
                }
            };
        }

        public async Task<ReporteAsistenciaDiariaDto?> GetAsistenciaDiariaAsync(DateTime fecha)
        {
            var asistencias = await _reporteRepository.GetAsistenciasPorFechaAsync(fecha);
            if (!asistencias.Any()) return null;

            var reporte = asistencias
                .GroupBy(a => a.Inscripcion.Materia.Nombre)
                .Select(g => new AsistenciaPorMateriaDto
                {
                    Materia = g.Key,
                    TotalEstudiantes = g.Count(),
                    Asistieron = g.Count(a => a.Asistio),
                    Faltaron = g.Count(a => !a.Asistio),
                    PorcentajeAsistencia = g.Count() > 0 ? Math.Round((decimal)(g.Count(a => a.Asistio) * 100.0 / g.Count()), 2) : 0,
                    Detalles = g.Select(a => new DetalleAsistenciaDto
                    {
                        Estudiante = $"{a.Inscripcion.Estudiante.Nombre} {a.Inscripcion.Estudiante.Apellido}",
                        Matricula = a.Inscripcion.Estudiante.Matricula,
                        Asistio = a.Asistio ? "Sí" : "No"
                    }).OrderBy(d => d.Estudiante).ToList()
                })
                .OrderBy(r => r.Materia)
                .ToList();

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
                        : 0
                }
            };
        }

        public async Task<RendimientoEstudianteDto?> GetRendimientoEstudianteAsync(int estudianteId)
        {
            var estudiante = await _reporteRepository.GetEstudianteByIdAsync(estudianteId);
            if (estudiante == null) return null;

            var historialInscripciones = await _reporteRepository.GetHistorialEstudianteAsync(estudianteId);

            var historial = historialInscripciones.Select(i => new HistorialAcademicoDto
            {
                Periodo = i.Periodo.NombrePeriodo,
                Materia = i.Materia.Nombre,
                PromedioNotas = i.Calificaciones.Any() ? Math.Round((decimal)i.Calificaciones.Average(c => c.Nota ?? 0), 2) : 0m,
                TotalCalificaciones = i.Calificaciones.Count(),
                PorcentajeAsistencia = i.Asistencias.Any() ? Math.Round((decimal)(i.Asistencias.Count(a => a.Asistio) * 100.0 / i.Asistencias.Count), 2) : 0m,
                TotalClases = i.Asistencias.Count
            }).OrderBy(r => r.Periodo).ThenBy(r => r.Materia).ToList();

            return new RendimientoEstudianteDto
            {
                Estudiante = MapToEstudianteReadDto(estudiante),
                FechaGeneracion = DateTime.Now,
                HistorialAcademico = historial,
                ResumenGeneral = historial.Any()
                    ? new ResumenRendimientoDto
                    {
                        PromedioGeneral = Math.Round(historial.Average(r => r.PromedioNotas), 2),
                        AsistenciaPromedio = Math.Round(historial.Average(r => r.PorcentajeAsistencia), 2),
                        TotalMaterias = historial.Count
                    }
                    : null
            };
        }

        public async Task<EstadisticasGeneralesDto> GetEstadisticasGeneralesAsync()
        {
            var totalEstudiantes = await _reporteRepository.GetTotalEstudiantesAsync();
            var estudiantesActivos = await _reporteRepository.GetEstudiantesActivosAsync();
            var totalProfesores = await _reporteRepository.GetTotalProfesoresAsync();
            var totalMaterias = await _reporteRepository.GetTotalMateriasAsync();
            var totalInscripciones = await _reporteRepository.GetTotalInscripcionesAsync();
            var periodosActivos = await _reporteRepository.GetPeriodosActivosAsync();
            var promedioGeneral = await _reporteRepository.GetPromedioGeneralCalificacionesAsync();

            // Porcentaje asistencia promedio
            var asistencias = await _reporteRepository.GetAsistenciasPorFechaAsync(DateTime.MinValue); // opcional, si quieres calcular porcentaje
            decimal porcentajeAsistencia = asistencias.Any() ? Math.Round((decimal)(asistencias.Count(a => a.Asistio) * 100.0 / asistencias.Count), 2) : 0m;

            return new EstadisticasGeneralesDto
            {
                TotalEstudiantes = totalEstudiantes,
                EstudiantesActivos = estudiantesActivos,
                TotalProfesores = totalProfesores,
                TotalMaterias = totalMaterias,
                TotalInscripciones = totalInscripciones,
                PeriodosActivos = periodosActivos,
                PromedioGeneralSistema = Math.Round(promedioGeneral, 2),
                PorcentajeAsistenciaGeneral = porcentajeAsistencia
            };
        }

        public async Task<IEnumerable<EstudianteConCalificacionesDto>> GetEstudiantesConCalificacionesAsync(int periodoId)
        {
            var estudiantes = await _reporteRepository.GetEstudiantesConCalificacionesPorPeriodoAsync(periodoId);

            return estudiantes.Select(e => new EstudianteConCalificacionesDto
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
            });
        }

        // ---------------- Mapeos ----------------
        private EstudianteReadDto MapToEstudianteReadDto(Estudiante e) =>
            new()
            {
                EstudianteID = e.EstudianteID,
                Nombre = e.Nombre,
                Apellido = e.Apellido,
                Matricula = e.Matricula,
                FechaCreacion = e.FechaCreacion,
                Activo = e.Activo
            };

        private PeriodoReadDto MapToPeriodoReadDto(Periodo p) =>
            new()
            {
                PeriodoID = p.PeriodoID,
                NombrePeriodo = p.NombrePeriodo,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            };

        private CalificacionReadDto MapToCalificacionReadDto(Calificacion c) =>
            new()
            {
                CalificacionID = c.CalificacionID,
                InscripcionID = c.InscripcionID,
                Nota = c.Nota,
                Literal = c.Literal.ToString(),
                FechaCreacion = c.FechaCreacion
            };

    }
}



