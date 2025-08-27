using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMateriaRepository
    {
        Task<(IEnumerable<Materia> Data, int TotalRecords)> GetMateriasAsync(string? filtro = null, int page = 1, int pageSize = 10);
        Task<Materia?> GetMateriaByIdAsync(int id);
        Task AddAsync(Materia materia);
        Task UpdateAsync(Materia materia);
        Task DeleteAsync(Materia materia);
        Task<bool> HasInscripcionesAsync(int materiaId);
        Task<IEnumerable<Materia>> GetMateriasByProfesorAsync(int profesorId);
        Task SaveChangesAsync();
    }
}
