using Application.DTOs.Reportes;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly IReporteService _reporteService;

        public ReportesController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        [HttpGet("boletin-estudiante/{estudianteId:int}/{periodoId:int}")]
        public async Task<ActionResult<BoletinEstudianteDto?>> GetBoletinEstudiante(int estudianteId, int periodoId)
        {
            var reporte = await _reporteService.GetBoletinEstudianteAsync(estudianteId, periodoId);
            return reporte is not null ? Ok(reporte) : NotFound();
        }

        [HttpGet("resumen-curso/{periodoId:int}")]
        public async Task<ActionResult<ResumenCursoDto?>> GetResumenCurso(int periodoId)
        {
            var reporte = await _reporteService.GetResumenCursoAsync(periodoId);
            return reporte is not null ? Ok(reporte) : NotFound();
        }

        [HttpGet("asistencia-diaria/{fecha:datetime}")]
        public async Task<ActionResult<ReporteAsistenciaDiariaDto?>> GetAsistenciaDiaria(DateTime fecha)
        {
            var reporte = await _reporteService.GetAsistenciaDiariaAsync(fecha);
            return reporte is not null ? Ok(reporte) : NotFound();
        }

        [HttpGet("rendimiento-estudiante/{estudianteId:int}")]
        public async Task<ActionResult<RendimientoEstudianteDto?>> GetRendimientoEstudiante(int estudianteId)
        {
            var reporte = await _reporteService.GetRendimientoEstudianteAsync(estudianteId);
            return reporte is not null ? Ok(reporte) : NotFound();
        }

        [HttpGet("estadisticas-generales")]
        public async Task<ActionResult<EstadisticasGeneralesDto>> GetEstadisticasGenerales()
        {
            var reporte = await _reporteService.GetEstadisticasGeneralesAsync();
            return Ok(reporte);
        }

        [HttpGet("estudiantes-calificaciones/{periodoId:int}")]
        public async Task<ActionResult<IEnumerable<EstudianteConCalificacionesDto>>> GetEstudiantesConCalificaciones(int periodoId)
        {
            var result = await _reporteService.GetEstudiantesConCalificacionesAsync(periodoId);
            return Ok(result);
        }
    }
}
