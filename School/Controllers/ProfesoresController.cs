using Application.DTOs.Profesor;
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
    public class ProfesoresController : ControllerBase
    {
        private readonly IProfesorService _profesorService;

        public ProfesoresController(IProfesorService profesorService)
        {
            _profesorService = profesorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfesorReadDto>>> GetProfesores([FromQuery] string? filtro, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (data, pagination) = await _profesorService.GetProfesoresAsync(filtro, page, pageSize);
            Response.Headers["X-Pagination-TotalRecords"] = pagination.TotalRecords.ToString();
            Response.Headers["X-Pagination-PageSize"] = pagination.PageSize.ToString();
            Response.Headers["X-Pagination-CurrentPage"] = pagination.CurrentPage.ToString();
            Response.Headers["X-Pagination-TotalPages"] = pagination.TotalPages.ToString();
            return Ok(data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProfesorReadDto?>> GetProfesorById(int id)
        {
            var profesor = await _profesorService.GetProfesorByIdAsync(id);
            return profesor is not null ? Ok(profesor) : NotFound();
        }

        [HttpGet("{id:int}/materias")]
        public async Task<ActionResult<ProfesorConMateriasDto?>> GetProfesorConMaterias(int id)
        {
            var data = await _profesorService.GetProfesorConMateriasAsync(id);
            return data is not null ? Ok(data) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ProfesorReadDto>> CreateProfesor([FromBody] ProfesorCreateDto dto)
        {
            if (await _profesorService.ExisteCorreoAsync(dto.Correo))
                return Conflict("Ya existe un profesor con ese correo.");

            var created = await _profesorService.CreateProfesorAsync(dto);
            return CreatedAtAction(nameof(GetProfesorById), new { id = created.ProfesorID }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProfesor(int id, [FromBody] ProfesorUpdateDto dto)
        {
            if (await _profesorService.ExisteCorreoAsync(dto.Correo, id))
                return Conflict("Ya existe un profesor con ese correo.");

            var updated = await _profesorService.UpdateProfesorAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProfesor(int id)
        {
            var (success, _, _) = await _profesorService.DeleteProfesorAsync(id);
            return success ? NoContent() : NotFound();
        }

    }
}
