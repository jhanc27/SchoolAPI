using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IInscripcionRepository
    {
        Task<(IEnumerable<Inscripcion> Data, int TotalCount)> GetInscripcionesAsync(
            int? estudianteId = null,
            int? materiaId = null,
            int? periodoId = null,
            int page = 1,
            int pageSize = 10
         );

        Task<Inscripcion?> GetByIdAsync(int id);
        Task<IEnumerable<Inscripcion>> GetByEstudianteAsync(int estudianteId);
        Task<IEnumerable<Inscripcion>> GetByMateriaAsync(int materiaId);
        Task<IEnumerable<Inscripcion>> GetByPeriodoAsync(int periodoId);
        Task AddAsync(Inscripcion inscripcion);
        Task UpdateAsync(Inscripcion inscripcion);
        Task DeleteAsync(Inscripcion inscripcion);
        Task<bool> ExisteInscripcionAsync(int estudianteId, int materiaId, int periodoId, int? excludeId = null);
        Task<bool> ValidarInscripcionAsync(int estudianteId, int materiaId, int periodoId);
    }
}
