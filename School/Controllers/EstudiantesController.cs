using Application.DTOs.Estudiante;
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
    public class EstudiantesController : ControllerBase
    {
        private readonly IEstudianteService _estudianteService;

        public EstudiantesController(IEstudianteService estudianteService)
        {
            _estudianteService = estudianteService;
        }

        // Obtener estudiantes con filtro y paginación
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstudianteReadDto>>> GetEstudiantes(
            [FromQuery] string? filtro,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var (data, pagination) = await _estudianteService.GetEstudiantesAsync(filtro, page, pageSize);

            Response.Headers["X-Pagination-TotalRecords"] = pagination.TotalRecords.ToString();
            Response.Headers["X-Pagination-PageSize"] = pagination.PageSize.ToString();
            Response.Headers["X-Pagination-CurrentPage"] = pagination.CurrentPage.ToString();
            Response.Headers["X-Pagination-TotalPages"] = pagination.TotalPages.ToString();

            return Ok(data);
        }

        // Obtener estudiante por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EstudianteReadDto?>> GetEstudianteById(int id)
        {
            var estudiante = await _estudianteService.GetEstudianteByIdAsync(id);
            return estudiante is not null ? Ok(estudiante) : NotFound();
        }

        // Crear estudiante
        [HttpPost]
        public async Task<ActionResult<EstudianteReadDto>> CreateEstudiante([FromBody] EstudianteCreateDto dto)
        {
            if (await _estudianteService.ExisteMatriculaAsync(dto.Matricula))
                return Conflict("Ya existe un estudiante con esa matrícula.");

            var nuevoEstudiante = await _estudianteService.CreateEstudianteAsync(dto);
            return CreatedAtAction(nameof(GetEstudianteById), new { id = nuevoEstudiante.EstudianteID }, nuevoEstudiante);
        }

        // Actualizar estudiante
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEstudiante(int id, [FromBody] EstudianteUpdateDto dto)
        {
            if (await _estudianteService.ExisteMatriculaAsync(dto.Matricula, id))
                return Conflict("Ya existe un estudiante con esa matrícula.");

            var updated = await _estudianteService.UpdateEstudianteAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        // Eliminar estudiante
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEstudiante(int id)
        {
            var (success, _, _) = await _estudianteService.DeleteEstudianteAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
