using Application.DTOs.Periodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IPeriodoService
    {
        Task<IEnumerable<PeriodoReadDto>> GetPeriodosAsync();
        Task<PeriodoReadDto?> GetPeriodoActivoAsync();
        Task<PeriodoReadDto?> GetPeriodoByIdAsync(int id);
        Task<PeriodoReadDto> CreatePeriodoAsync(PeriodoCreateDto periodoDto);
        Task<bool> UpdatePeriodoAsync(int id, PeriodoUpdateDto periodoDto);
        Task<bool> ValidarFechasPeriodoAsync(DateTime fechaInicio, DateTime fechaFin, int? excludeId = null);
        Task<bool> DesactivarPeriodosAnterioresAsync(int periodoId);
        Task<bool> DeletePeriodoAsync(int periodoId);
    }
}
