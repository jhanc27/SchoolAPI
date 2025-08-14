using Application.DTOs.Inscripcion;
using Application.DTOs.Paginacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInscripcionService
    {
        Task<(IEnumerable<InscripcionReadDto> Data, PaginationMetadata Pagination)>
       GetInscripcionesAsync(int? estudianteId = null, int? materiaId = null, int? periodoId = null, int page = 1, int pageSize = 10);

        Task<InscripcionReadDto?> GetInscripcionByIdAsync(int id);
        Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByEstudianteAsync(int estudianteId);
        Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByMateriaAsync(int materiaId);
        Task<IEnumerable<InscripcionReadDto>> GetInscripcionesByPeriodoAsync(int periodoId);
        Task<InscripcionReadDto> CreateInscripcionAsync(InscripcionCreateDto inscripcionDto);
        Task<bool> UpdateInscripcionAsync(int id, InscripcionUpdateDto inscripcionDto);
        Task<bool> DeleteInscripcionAsync(int id);
        Task<bool> ExisteInscripcionAsync(int estudianteId, int materiaId, int periodoId, int? excludeId = null);
        Task<bool> ValidarInscripcionAsync(int estudianteId, int materiaId, int periodoId);
    }
}
