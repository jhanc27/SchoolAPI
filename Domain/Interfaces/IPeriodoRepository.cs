using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPeriodoRepository
    {
        Task<List<Periodo>> GetPeriodosActivosAsync();
        Task<Periodo?> GetPeriodoActivoAsync();
        Task<Periodo?> GetPeriodoByIdAsync(int id);
        Task AddPeriodoAsync(Periodo periodo);
        Task UpdatePeriodoAsync(Periodo periodo);
        Task DeletePeriodoAsync(Periodo periodo);
        Task<bool> ExisteSuperposicionAsync(DateTime fechaInicio, DateTime fechaFin, int? excludeId = null);
        Task<List<Periodo>> GetPeriodosAnterioresAsync(int periodoId);
    }
}
