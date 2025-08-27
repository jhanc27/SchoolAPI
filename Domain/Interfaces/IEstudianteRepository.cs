using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEstudianteRepository
    {
        Task<IEnumerable<Estudiante>> GetAllAsync(string? filtro = null, int page = 1, int pageSize = 10);
        Task<int> CountAsync(string? filtro = null);
        Task<Estudiante?> GetByIdAsync(int id);
        Task AddAsync(Estudiante estudiante);
        Task UpdateAsync(Estudiante estudiante);
        Task DeleteAsync(Estudiante estudiante);
        Task<bool> ExisteMatriculaAsync(string matricula, int? excludeId = null);
        Task<bool> TieneInscripcionesAsync(int estudianteId);
        Task<bool> TieneCalificacionesAsync(int estudianteId);
    }
}
