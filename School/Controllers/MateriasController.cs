using Application.DTOs.Materia;
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
    public class MateriasController : ControllerBase
    {
        private readonly IMateriaService _materiaService;

        public MateriasController(IMateriaService materiaService)
        {
            _materiaService = materiaService;
        }

        // Obtener todas las materias con paginación y filtro opcional
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MateriaReadDto>>> GetMaterias([FromQuery] string? filtro, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (data, pagination) = await _materiaService.GetMateriasAsync(filtro, page, pageSize);
            Response.Headers["X-Pagination-TotalRecords"] = pagination.TotalRecords.ToString();
            Response.Headers["X-Pagination-PageSize"] = pagination.PageSize.ToString();
            Response.Headers["X-Pagination-CurrentPage"] = pagination.CurrentPage.ToString();
            Response.Headers["X-Pagination-TotalPages"] = pagination.TotalPages.ToString();
            return Ok(data);
        }

        // Obtener materia por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MateriaReadDto?>> GetMateriaById(int id)
        {
            var materia = await _materiaService.GetMateriaByIdAsync(id);
            return materia is not null ? Ok(materia) : NotFound();
        }

        // Obtener materias por profesor
        [HttpGet("profesor/{profesorId:int}")]
        public async Task<ActionResult<IEnumerable<MateriaReadDto>>> GetMateriasByProfesor(int profesorId)
        {
            var data = await _materiaService.GetMateriasByProfesorAsync(profesorId);
            return Ok(data);
        }

        // Crear materia
        [HttpPost]
        public async Task<ActionResult<MateriaReadDto>> CreateMateria([FromBody] MateriaCreateDto dto)
        {
            var created = await _materiaService.CreateMateriaAsync(dto);
            return CreatedAtAction(nameof(GetMateriaById), new { id = created.MateriaID }, created);
        }

        // Actualizar materia
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMateria(int id, [FromBody] MateriaUpdateDto dto)
        {
            var updated = await _materiaService.UpdateMateriaAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        // Eliminar materia
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMateria(int id)
        {
            var (success, message) = await _materiaService.DeleteMateriaAsync(id);
            return success ? NoContent() : BadRequest(message);
        }

    }
}
