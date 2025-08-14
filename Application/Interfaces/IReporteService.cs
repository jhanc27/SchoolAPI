using Application.DTOs.Reportes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IReporteService
    {
        Task<BoletinEstudianteDto?> GetBoletinEstudianteAsync(int estudianteId, int periodoId);
        Task<ResumenCursoDto?> GetResumenCursoAsync(int periodoId);
        Task<ReporteAsistenciaDiariaDto?> GetAsistenciaDiariaAsync(DateTime fecha);
        Task<RendimientoEstudianteDto?> GetRendimientoEstudianteAsync(int estudianteId);
        Task<EstadisticasGeneralesDto> GetEstadisticasGeneralesAsync();
        Task<IEnumerable<EstudianteConCalificacionesDto>> GetEstudiantesConCalificacionesAsync(int periodoId);
    }
}

