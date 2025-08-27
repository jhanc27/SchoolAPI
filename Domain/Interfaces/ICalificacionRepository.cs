using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICalificacionRepository
    {
        Task<IEnumerable<Calificacion>> GetByInscripcionAsync(int inscripcionId);
        Task<IEnumerable<Calificacion>> GetByEstudianteAsync(int estudianteId);
        Task<Calificacion?> GetByIdAsync(int id);
        Task AddAsync(Calificacion calificacion);
        Task UpdateAsync(Calificacion calificacion);
        Task DeleteAsync(Calificacion calificacion);
        Task<bool> ExistsAsync(int inscripcionId, int? excludeId = null);
    }
}
