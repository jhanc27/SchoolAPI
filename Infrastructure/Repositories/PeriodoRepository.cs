using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PeriodoRepository : IPeriodoRepository
    {
        private readonly ApplicationDbContext _context;

        public PeriodoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Periodo>> GetPeriodosActivosAsync() =>
            await _context.Periodos.Where(p => p.Activo).OrderByDescending(p => p.FechaInicio).ToListAsync();

        public async Task<Periodo?> GetPeriodoActivoAsync() =>
            await _context.Periodos.FirstOrDefaultAsync(p => p.Activo);

        public async Task<Periodo?> GetPeriodoByIdAsync(int id) =>
            await _context.Periodos.FirstOrDefaultAsync(p => p.PeriodoID == id);

        public async Task AddPeriodoAsync(Periodo periodo)
        {
            _context.Periodos.Add(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePeriodoAsync(Periodo periodo)
        {
            _context.Periodos.Update(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePeriodoAsync(Periodo periodo)
        {
            _context.Periodos.Remove(periodo);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteSuperposicionAsync(DateTime fechaInicio, DateTime fechaFin, int? excludeId = null)
        {
            var query = _context.Periodos.Where(p => p.Activo);
            if (excludeId.HasValue)
                query = query.Where(p => p.PeriodoID != excludeId.Value);

            return await query.AnyAsync(p =>
                (fechaInicio >= p.FechaInicio && fechaInicio <= p.FechaFin) ||
                (fechaFin >= p.FechaInicio && fechaFin <= p.FechaFin) ||
                (fechaInicio <= p.FechaInicio && fechaFin >= p.FechaFin));
        }

        public async Task<List<Periodo>> GetPeriodosAnterioresAsync(int periodoId) =>
            await _context.Periodos.Where(p => p.PeriodoID != periodoId && p.Activo).ToListAsync();
    }
}
