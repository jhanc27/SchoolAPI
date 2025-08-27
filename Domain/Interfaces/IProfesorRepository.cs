using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProfesorRepository
    {
        Task<IEnumerable<Profesor>> GetAllAsync(string? filtro = null, int page = 1, int pageSize = 10);
        Task<int> CountAsync(string? filtro = null);
        Task<Profesor?> GetByIdAsync(int id);
        Task AddAsync(Profesor profesor);
        Task UpdateAsync(Profesor profesor);
        Task DeleteAsync(Profesor profesor);
        Task<bool> ExisteCorreoAsync(string correo, int? excludeId = null);
        Task<Profesor?> GetProfesorConMateriasAsync(int id);
    }
}
