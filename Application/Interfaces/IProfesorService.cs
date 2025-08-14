using Application.DTOs.Paginacion;
using Application.DTOs.Profesor;
using Application.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IProfesorService
    {
        Task<(IEnumerable<ProfesorReadDto> Data, PaginationMetadata Pagination)>
         GetProfesoresAsync(string? filtro = null, int page = 1, int pageSize = 10);

        Task<ProfesorReadDto?> GetProfesorByIdAsync(int id);
        Task<ProfesorReadDto> CreateProfesorAsync(ProfesorCreateDto profesorDto);
        Task<bool> UpdateProfesorAsync(int id, ProfesorUpdateDto profesorDto);
        Task<(bool Success, string Message, bool SoftDelete)> DeleteProfesorAsync(int id);
        Task<bool> ExisteCorreoAsync(string correo, int? excludeId = null);
        Task<ProfesorConMateriasDto?> GetProfesorConMateriasAsync(int id);
    }
}
