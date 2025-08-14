using Application.DTOs.Asistencia;
using Application.DTOs.Paginacion;
using Application.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAsistenciaService
    {
        Task<(IEnumerable<AsistenciaReadDto> Data, PaginationMetadata Pagination)>
        GetAsistenciasAsync(DateTime? fecha = null, int? inscripcionId = null, int page = 1, int pageSize = 10);

        Task<IEnumerable<AsistenciaReadDto>> GetAsistenciasByInscripcionAsync(int inscripcionId);
        Task<IEnumerable<AsistenciaReadDto>> GetAsistenciasByFechaAsync(DateTime fecha);
        Task<AsistenciaReadDto?> GetAsistenciaByIdAsync(int id);
        Task<AsistenciaReadDto> CreateAsistenciaAsync(AsistenciaCreateDto asistenciaDto);
        Task<bool> UpdateAsistenciaAsync(int id, AsistenciaUpdateDto asistenciaDto);
        Task<bool> DeleteAsistenciaAsync(int id);
        Task<bool> ExisteAsistenciaAsync(int inscripcionId, DateTime fecha, int? excludeId = null);
        Task<ReporteAsistenciaDto> GetReporteAsistenciaAsync(int estudianteId, int materiaId);
    }
}
