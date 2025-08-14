using Application.DTOs.Periodo;
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
    public class PeriodoService : IPeriodoService
    {
        private readonly ApplicationDbContext _context;

        public PeriodoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PeriodoReadDto>> GetPeriodosAsync()
        {
            var periodos = await _context.Periodos
                .Where(p => p.Activo)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();

            return periodos.Select(MapToReadDto);
        }

        public async Task<PeriodoReadDto?> GetPeriodoActivoAsync()
        {
            var periodo = await _context.Periodos.FirstOrDefaultAsync(p => p.Activo);
            return periodo is null ? null : MapToReadDto(periodo);
        }

        public async Task<PeriodoReadDto?> GetPeriodoByIdAsync(int id)
        {
            var periodo = await _context.Periodos.FirstOrDefaultAsync(p => p.PeriodoID == id);
            return periodo is null ? null : MapToReadDto(periodo);
        }

        public async Task<PeriodoReadDto> CreatePeriodoAsync(PeriodoCreateDto periodoDto)
        {
            ValidarDatosPeriodo(periodoDto.NombrePeriodo, periodoDto.FechaInicio, periodoDto.FechaFin);

            if (!await ValidarFechasPeriodoAsync(periodoDto.FechaInicio, periodoDto.FechaFin))
                throw new InvalidOperationException("Las fechas se superponen con otro período existente");

            var periodo = new Periodo
            {
                NombrePeriodo = periodoDto.NombrePeriodo.Trim(),
                FechaInicio = periodoDto.FechaInicio,
                FechaFin = periodoDto.FechaFin,
                Activo = periodoDto.Activo,
                FechaCreacion = DateTime.Now
            };

            _context.Periodos.Add(periodo);
            await _context.SaveChangesAsync();

            return MapToReadDto(periodo);
        }

        public async Task<bool> UpdatePeriodoAsync(int id, PeriodoUpdateDto periodoDto)
        {
            var periodo = await _context.Periodos.FindAsync(id);
            if (periodo == null) return false;

            ValidarDatosPeriodo(periodoDto.NombrePeriodo, periodoDto.FechaInicio, periodoDto.FechaFin);

            if (!await ValidarFechasPeriodoAsync(periodoDto.FechaInicio, periodoDto.FechaFin, id))
                throw new InvalidOperationException("Las fechas se superponen con otro período existente");

            periodo.NombrePeriodo = periodoDto.NombrePeriodo.Trim();
            periodo.FechaInicio = periodoDto.FechaInicio;
            periodo.FechaFin = periodoDto.FechaFin;
            periodo.Activo = periodoDto.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidarFechasPeriodoAsync(DateTime fechaInicio, DateTime fechaFin, int? excludeId = null)
        {
            var query = _context.Periodos.Where(p => p.Activo);

            if (excludeId.HasValue)
                query = query.Where(p => p.PeriodoID != excludeId.Value);

            var periodosSuperpuestos = await query
                .Where(p => (fechaInicio >= p.FechaInicio && fechaInicio <= p.FechaFin) ||
                            (fechaFin >= p.FechaInicio && fechaFin <= p.FechaFin) ||
                            (fechaInicio <= p.FechaInicio && fechaFin >= p.FechaFin))
                .AnyAsync();

            return !periodosSuperpuestos;
        }

        public async Task<bool> DesactivarPeriodosAnterioresAsync(int periodoId)
        {
            var periodosAnteriores = await _context.Periodos
                .Where(p => p.PeriodoID != periodoId && p.Activo)
                .ToListAsync();

            foreach (var p in periodosAnteriores)
                p.Activo = false;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePeriodoAsync(int periodoId)
        {
            var periodo = await _context.Periodos.FindAsync(periodoId);
            if (periodo == null) return false;

            _context.Periodos.Remove(periodo);
            await _context.SaveChangesAsync();
            return true;
        }

        // -------------------------- Helpers --------------------------

        private PeriodoReadDto MapToReadDto(Periodo p) =>
            new PeriodoReadDto
            {
                PeriodoID = p.PeriodoID,
                NombrePeriodo = p.NombrePeriodo,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                Activo = p.Activo,
                FechaCreacion = p.FechaCreacion
            };

        private void ValidarDatosPeriodo(string nombrePeriodo, DateTime fechaInicio, DateTime fechaFin)
        {
            if (string.IsNullOrWhiteSpace(nombrePeriodo))
                throw new ArgumentException("El nombre del período es requerido");

            if (nombrePeriodo.Length > 20)
                throw new ArgumentException("El nombre del período no puede exceder 20 caracteres");

            if (fechaFin <= fechaInicio)
                throw new ArgumentException("La fecha de fin debe ser posterior a la fecha de inicio");
        }
    }
}
