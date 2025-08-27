using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IAsistenciaRepository
    {
        Task<Asistencia?> GetByIdAsync(int id);
        Task<IEnumerable<Asistencia>> GetByInscripcionAsync(int inscripcionId);
        Task<IEnumerable<Asistencia>> GetByFechaAsync(DateTime fecha);
        Task<IEnumerable<Asistencia>> GetByEstudianteAsync(int estudianteId);
        Task<(IEnumerable<Asistencia> Data, int TotalRecords)> GetAsync(
            DateTime? fecha = null,
            int? inscripcionId = null,
            int page = 1,
            int pageSize = 10);

        Task AddAsync(Asistencia asistencia);
        Task UpdateAsync(Asistencia asistencia);
        Task DeleteAsync(Asistencia asistencia);
        Task<bool> ExisteAsync(int inscripcionId, DateTime fecha, int? excludeId = null);

        Task SaveChangesAsync();
    }
}
