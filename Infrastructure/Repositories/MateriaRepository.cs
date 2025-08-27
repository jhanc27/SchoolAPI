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
    public class MateriaRepository : IMateriaRepository
    {
        private readonly ApplicationDbContext _context;

        public MateriaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Materia> Data, int TotalRecords)> GetMateriasAsync(string? filtro = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Materias
                .Include(m => m.Profesor)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = $"%{filtro.ToLower()}%";
                query = query.Where(m =>
                    (m.Nombre != null && EF.Functions.Like(m.Nombre.ToLower(), filtro)) ||
                    (m.Profesor != null && m.Profesor.Nombre != null && EF.Functions.Like(m.Profesor.Nombre.ToLower(), filtro)) ||
                    (m.Profesor != null && m.Profesor.Apellido != null && EF.Functions.Like(m.Profesor.Apellido.ToLower(), filtro))
                );
            }

            var totalRecords = await query.CountAsync();

            var materias = await query
                .OrderBy(m => m.Nombre)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (materias, totalRecords);
        }

        public async Task<Materia?> GetMateriaByIdAsync(int id)
        {
            return await _context.Materias
                .Include(m => m.Profesor)
                .FirstOrDefaultAsync(m => m.MateriaID == id);
        }

        public async Task AddAsync(Materia materia)
        {
            await _context.Materias.AddAsync(materia);
        }

        public async Task UpdateAsync(Materia materia)
        {
            _context.Materias.Update(materia);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Materia materia)
        {
            _context.Materias.Remove(materia);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasInscripcionesAsync(int materiaId)
        {
            return await _context.Inscripciones.AnyAsync(i => i.MateriaID == materiaId);
        }

        public async Task<IEnumerable<Materia>> GetMateriasByProfesorAsync(int profesorId)
        {
            return await _context.Materias
                .Include(m => m.Profesor)
                .Where(m => m.ProfesorID == profesorId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
