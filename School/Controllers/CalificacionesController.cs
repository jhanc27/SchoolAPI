using Application.DTOs.Calificacion;
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
    public class CalificacionesController : ControllerBase
    {
        private readonly ICalificacionService _calificacionService;

        public CalificacionesController(ICalificacionService calificacionService)
        {
            _calificacionService = calificacionService;
        }

        [HttpGet("inscripcion/{inscripcionId}")]
        public async Task<ActionResult<IEnumerable<CalificacionReadDto>>> GetCalificacionesByInscripcion(int inscripcionId)
        {
            var result = await _calificacionService.GetCalificacionesByInscripcionAsync(inscripcionId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CalificacionReadDto?>> GetCalificacionById(int id)
        {
            var result = await _calificacionService.GetCalificacionByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }


        [HttpGet("estudiante/{estudianteId}")]
        public async Task<ActionResult<IEnumerable<CalificacionReadDto>>> GetCalificacionesByEstudiante(int estudianteId)
        {
            var result = await _calificacionService.GetCalificacionesByEstudianteAsync(estudianteId);
            return Ok(result);
        }

        [HttpGet("literal-por-nota/{nota}")]
        public ActionResult<string> CalcularLiteralPorNota(decimal nota)
        {
            var literalEnum = _calificacionService.CalcularLiteralPorNota(nota);
            return Ok(literalEnum.ToString()); // Convierte enum → string
        }

        [HttpGet("nota-por-literal/{literal}")]
        public ActionResult<decimal> CalcularNotaPorLiteral(LiteralCalificacion literal)
        {
            var nota = _calificacionService.CalcularNotaPorLiteral(literal);
            return Ok(nota);
        }

        [HttpGet("validar-consistencia")]
        public ActionResult<bool> ValidarConsistenciaNotaLiteral(decimal nota, LiteralCalificacion literal)
        {
            var valido = _calificacionService.ValidarConsistenciaNotaLiteral(nota, literal);
            return Ok(valido);
        }

        [HttpPost]
        public async Task<ActionResult<CalificacionReadDto>> CreateCalificacion([FromBody] CalificacionCreateDto dto)
        {
            var created = await _calificacionService.CreateCalificacionAsync(dto);
            return CreatedAtAction(nameof(GetCalificacionById), new { id = created.CalificacionID }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCalificacion(int id, [FromBody] CalificacionUpdateDto dto)
        {
            bool updated = await _calificacionService.UpdateCalificacionAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalificacion(int id)
        {
            bool deleted = await _calificacionService.DeleteCalificacionAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

    }
}
