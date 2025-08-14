using Application.DTOs.Materia;
using Application.DTOs.Paginacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMateriaService
    {
        Task<(IEnumerable<MateriaReadDto> Data, PaginationMetadata Pagination)>
        GetMateriasAsync(string? filtro = null, int page = 1, int pageSize = 10);

        Task<MateriaReadDto?> GetMateriaByIdAsync(int id);
        Task<MateriaReadDto> CreateMateriaAsync(MateriaCreateDto materiaDto);
        Task<bool> UpdateMateriaAsync(int id, MateriaUpdateDto materiaDto);
        Task<(bool Success, string Message)> DeleteMateriaAsync(int id);
        Task<IEnumerable<MateriaReadDto>> GetMateriasByProfesorAsync(int profesorId);
    }
}
