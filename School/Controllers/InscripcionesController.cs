using Application.DTOs.Inscripcion;
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
    public class InscripcionesController : ControllerBase
    {
        private readonly IInscripcionService _inscripcionService;

        public InscripcionesController(IInscripcionService inscripcionService)
        {
            _inscripcionService = inscripcionService;
        }

        // Obtener todas las inscripciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InscripcionReadDto>>> GetInscripciones()
        {
            var data = await _inscripcionService.GetInscripcionesAsync();
            return Ok(data);
        }

        // Obtener inscripción por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<InscripcionReadDto?>> GetInscripcionById(int id)
        {
            var inscripcion = await _inscripcionService.GetInscripcionByIdAsync(id);
            return inscripcion is not null ? Ok(inscripcion) : NotFound();
        }

        // Filtrar por estudiante
        [HttpGet("estudiante/{estudianteId:int}")]
        public async Task<ActionResult<IEnumerable<InscripcionReadDto>>> GetInscripcionesByEstudiante(int estudianteId)
        {
            var data = await _inscripcionService.GetInscripcionesByEstudianteAsync(estudianteId);
            return Ok(data);
        }

        // Filtrar por materia
        [HttpGet("materia/{materiaId:int}")]
        public async Task<ActionResult<IEnumerable<InscripcionReadDto>>> GetInscripcionesByMateria(int materiaId)
        {
            var data = await _inscripcionService.GetInscripcionesByMateriaAsync(materiaId);
            return Ok(data);
        }

        // Filtrar por período
        [HttpGet("periodo/{periodoId:int}")]
        public async Task<ActionResult<IEnumerable<InscripcionReadDto>>> GetInscripcionesByPeriodo(int periodoId)
        {
            var data = await _inscripcionService.GetInscripcionesByPeriodoAsync(periodoId);
            return Ok(data);
        }

        // Crear inscripción
        [HttpPost]
        public async Task<ActionResult<InscripcionReadDto>> CreateInscripcion([FromBody] InscripcionCreateDto dto)
        {
            if (await _inscripcionService.ExisteInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID))
                return Conflict("El estudiante ya está inscrito en esa materia para el período indicado.");

            if (!await _inscripcionService.ValidarInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID))
                return BadRequest("La inscripción no cumple las reglas de validación.");

            var created = await _inscripcionService.CreateInscripcionAsync(dto);
            return CreatedAtAction(nameof(GetInscripcionById), new { id = created.InscripcionID }, created);
        }

        // Actualizar inscripción
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateInscripcion(int id, [FromBody] InscripcionUpdateDto dto)
        {
            if (await _inscripcionService.ExisteInscripcionAsync(dto.EstudianteID, dto.MateriaID, dto.PeriodoID, id))
                return Conflict("El estudiante ya está inscrito en esa materia para el período indicado.");

            var updated = await _inscripcionService.UpdateInscripcionAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        // Eliminar inscripción
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInscripcion(int id)
        {
            var deleted = await _inscripcionService.DeleteInscripcionAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
