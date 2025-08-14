using Application.DTOs.Asistencia;
using Application.DTOs.Reportes;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace School.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AsistenciasController : ControllerBase
    {
        private readonly IAsistenciaService _asistenciaService;

        public AsistenciasController(IAsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
        }

        // Obtener todas las asistencias de una inscripción
        [HttpGet("inscripcion/{inscripcionId:int}")]
        public async Task<ActionResult<IEnumerable<AsistenciaReadDto>>> GetAsistenciasByInscripcion(int inscripcionId)
        {
            var asistencias = await _asistenciaService.GetAsistenciasByInscripcionAsync(inscripcionId);
            return Ok(asistencias);
        }

        // Obtener asistencias por fecha
        [HttpGet("fecha/{fecha:datetime}")]
        public async Task<ActionResult<IEnumerable<AsistenciaReadDto>>> GetAsistenciasByFecha(DateTime fecha)
        {
            var asistencias = await _asistenciaService.GetAsistenciasByFechaAsync(fecha);
            return Ok(asistencias);
        }

        // Obtener asistencia por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AsistenciaReadDto?>> GetAsistenciaById(int id)
        {
            var asistencia = await _asistenciaService.GetAsistenciaByIdAsync(id);
            return asistencia is not null ? Ok(asistencia) : NotFound();
        }

        // Obtener reporte de asistencia filtrado por estudiante y materia
        [HttpGet("reporte")]
        public async Task<ActionResult<ReporteAsistenciaDto>> GetReporteAsistencia([FromQuery] int estudianteId, [FromQuery] int materiaId)
        {
            var reporte = await _asistenciaService.GetReporteAsistenciaAsync(estudianteId, materiaId);
            return reporte is not null ? Ok(reporte) : NotFound();
        }

        // Crear nueva asistencia
        [HttpPost]
        public async Task<ActionResult<AsistenciaReadDto>> CreateAsistencia([FromBody] AsistenciaCreateDto dto)
        {
            if (await _asistenciaService.ExisteAsistenciaAsync(dto.InscripcionID, dto.Fecha))
                return Conflict("Ya existe una asistencia para esa inscripción y fecha.");

            var asistencia = await _asistenciaService.CreateAsistenciaAsync(dto);
            return CreatedAtAction(nameof(GetAsistenciaById), new { id = asistencia.AsistenciaID }, asistencia);
        }

        // Actualizar asistencia existente
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateAsistencia(int id, [FromBody] AsistenciaUpdateDto dto)
        {
            var asistenciaExistente = await _asistenciaService.GetAsistenciaByIdAsync(id);
            if (asistenciaExistente is null) return NotFound();

            if (await _asistenciaService.ExisteAsistenciaAsync(asistenciaExistente.InscripcionID, dto.Fecha, id))
                return Conflict("Ya existe una asistencia para esa inscripción y fecha.");

            var updated = await _asistenciaService.UpdateAsistenciaAsync(id, dto);
            return updated ? NoContent() : StatusCode(500, "No se pudo actualizar la asistencia.");
        }

        // Eliminar asistencia
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsistencia(int id)
        {
            var deleted = await _asistenciaService.DeleteAsistenciaAsync(id);
            return deleted ? NoContent() : NotFound();
        }

    }
}
