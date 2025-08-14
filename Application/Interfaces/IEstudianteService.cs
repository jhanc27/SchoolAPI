using Application.DTOs.Estudiante;
using Application.DTOs.Paginacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEstudianteService
    {
        Task<(IEnumerable<EstudianteReadDto> Data, PaginationMetadata Pagination)>
       GetEstudiantesAsync(string? filtro = null, int page = 1, int pageSize = 10);

        Task<EstudianteReadDto?> GetEstudianteByIdAsync(int id);
        Task<EstudianteReadDto> CreateEstudianteAsync(EstudianteCreateDto estudianteDto);
        Task<bool> UpdateEstudianteAsync(int id, EstudianteUpdateDto estudianteDto);
        Task<(bool Success, string Message, bool SoftDelete)> DeleteEstudianteAsync(int id);
        Task<bool> ExisteMatriculaAsync(string matricula, int? excludeId = null);
    }
}
