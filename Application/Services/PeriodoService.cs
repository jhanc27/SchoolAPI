using Application.DTOs.Periodo;
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
    public class PeriodoService : IPeriodoService
    {
        private readonly IPeriodoRepository _repository;

        public PeriodoService(IPeriodoRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PeriodoReadDto>> GetPeriodosAsync()
        {
            var periodos = await _repository.GetPeriodosActivosAsync();
            return periodos.Select(MapToReadDto);
        }

        public async Task<PeriodoReadDto?> GetPeriodoActivoAsync()
        {
            var periodo = await _repository.GetPeriodoActivoAsync();
            return periodo is null ? null : MapToReadDto(periodo);
        }

        public async Task<PeriodoReadDto?> GetPeriodoByIdAsync(int id)
        {
            var periodo = await _repository.GetPeriodoByIdAsync(id);
            return periodo is null ? null : MapToReadDto(periodo);
        }

        public async Task<PeriodoReadDto> CreatePeriodoAsync(PeriodoCreateDto dto)
        {
            ValidarDatosPeriodo(dto.NombrePeriodo, dto.FechaInicio, dto.FechaFin);

            if (!await _repository.ExisteSuperposicionAsync(dto.FechaInicio, dto.FechaFin))
                throw new InvalidOperationException("Las fechas se superponen con otro período existente");

            var periodo = new Periodo
            {
                NombrePeriodo = dto.NombrePeriodo.Trim(),
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                Activo = dto.Activo,
                FechaCreacion = DateTime.Now
            };

            await _repository.AddPeriodoAsync(periodo);
            return MapToReadDto(periodo);
        }

        public async Task<bool> UpdatePeriodoAsync(int id, PeriodoUpdateDto dto)
        {
            var periodo = await _repository.GetPeriodoByIdAsync(id);
            if (periodo == null) return false;

            ValidarDatosPeriodo(dto.NombrePeriodo, dto.FechaInicio, dto.FechaFin);

            if (!await _repository.ExisteSuperposicionAsync(dto.FechaInicio, dto.FechaFin, id))
                throw new InvalidOperationException("Las fechas se superponen con otro período existente");

            periodo.NombrePeriodo = dto.NombrePeriodo.Trim();
            periodo.FechaInicio = dto.FechaInicio;
            periodo.FechaFin = dto.FechaFin;
            periodo.Activo = dto.Activo;

            await _repository.UpdatePeriodoAsync(periodo);
            return true;
        }

        public async Task<bool> ValidarFechasPeriodoAsync(DateTime fechaInicio, DateTime fechaFin, int? excludeId = null)
        {
            return !await _repository.ExisteSuperposicionAsync(fechaInicio, fechaFin, excludeId);
        }

        public async Task<bool> DesactivarPeriodosAnterioresAsync(int periodoId)
        {
            var periodosAnteriores = await _repository.GetPeriodosAnterioresAsync(periodoId);
            foreach (var p in periodosAnteriores)
                p.Activo = false;

            foreach (var p in periodosAnteriores)
                await _repository.UpdatePeriodoAsync(p);

            return true;
        }

        public async Task<bool> DeletePeriodoAsync(int periodoId)
        {
            var periodo = await _repository.GetPeriodoByIdAsync(periodoId);
            if (periodo == null) return false;

            await _repository.DeletePeriodoAsync(periodo);
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
