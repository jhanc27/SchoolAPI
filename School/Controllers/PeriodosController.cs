using Application.DTOs.Periodo;
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
    public class PeriodosController : ControllerBase
    {
        private readonly IPeriodoService _periodoService;

        public PeriodosController(IPeriodoService periodoService)
        {
            _periodoService = periodoService;
        }

        // Obtener todos los periodos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeriodoReadDto>>> GetPeriodos()
        {
            var data = await _periodoService.GetPeriodosAsync();
            return Ok(data);
        }

        // Obtener el periodo activo
        [HttpGet("activo")]
        public async Task<ActionResult<PeriodoReadDto?>> GetPeriodoActivo()
        {
            var activo = await _periodoService.GetPeriodoActivoAsync();
            return activo is not null ? Ok(activo) : NotFound();
        }

        // Obtener periodo por ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PeriodoReadDto?>> GetPeriodoById(int id)
        {
            var periodo = await _periodoService.GetPeriodoByIdAsync(id);
            return periodo is not null ? Ok(periodo) : NotFound();
        }

        // Crear un nuevo periodo
        [HttpPost]
        public async Task<ActionResult<PeriodoReadDto>> CreatePeriodo([FromBody] PeriodoCreateDto dto)
        {
            var fechasValidas = await _periodoService.ValidarFechasPeriodoAsync(dto.FechaInicio, dto.FechaFin);
            if (!fechasValidas)
                return BadRequest("Las fechas del período se solapan con otro período existente.");

            var nuevoPeriodo = await _periodoService.CreatePeriodoAsync(dto);

            // Desactivar periodos anteriores
            await _periodoService.DesactivarPeriodosAnterioresAsync(nuevoPeriodo.PeriodoID);

            return CreatedAtAction(nameof(GetPeriodoById), new { id = nuevoPeriodo.PeriodoID }, nuevoPeriodo);
        }

        // Actualizar un periodo
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdatePeriodo(int id, [FromBody] PeriodoUpdateDto dto)
        {
            var fechasValidas = await _periodoService.ValidarFechasPeriodoAsync(dto.FechaInicio, dto.FechaFin, id);
            if (!fechasValidas)
                return BadRequest("Las fechas del período se solapan con otro período existente.");

            var updated = await _periodoService.UpdatePeriodoAsync(id, dto);
            return updated ? NoContent() : NotFound();
        }

        // Opcional: eliminar periodo si tu servicio lo soporta
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePeriodo(int id)
        {
            var deleted = await _periodoService.DeletePeriodoAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
