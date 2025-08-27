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
    public class ProfesorRepository : IProfesorRepository
    {
        private readonly ApplicationDbContext _context;

        public ProfesorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Profesor>> GetAllAsync(string? filtro = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Profesores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToLower();
                query = query.Where(p =>
                    (p.Nombre != null && p.Nombre.ToLower().Contains(filtro)) ||
                    (p.Apellido != null && p.Apellido.ToLower().Contains(filtro)) ||
                    (p.Correo != null && p.Correo.ToLower().Contains(filtro))
                );
            }

            return await query
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? filtro = null)
        {
            var query = _context.Profesores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.ToLower();
                query = query.Where(p =>
                    (p.Nombre != null && p.Nombre.ToLower().Contains(filtro)) ||
                    (p.Apellido != null && p.Apellido.ToLower().Contains(filtro)) ||
                    (p.Correo != null && p.Correo.ToLower().Contains(filtro))
                );
            }

            return await query.CountAsync();
        }

        public async Task<Profesor?> GetByIdAsync(int id)
        {
            return await _context.Profesores.FindAsync(id);
        }

        public async Task AddAsync(Profesor profesor)
        {
            _context.Profesores.Add(profesor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Profesor profesor)
        {
            _context.Profesores.Update(profesor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Profesor profesor)
        {
            _context.Profesores.Remove(profesor);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteCorreoAsync(string correo, int? excludeId = null)
        {
            var query = _context.Profesores.Where(p => p.Correo == correo.Trim().ToLower());
            if (excludeId.HasValue)
                query = query.Where(p => p.ProfesorID != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<Profesor?> GetProfesorConMateriasAsync(int id)
        {
            return await _context.Profesores
                .Include(p => p.Materias)
                .FirstOrDefaultAsync(p => p.ProfesorID == id);
        }
    }
}
