using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IReporteRepository
    {
        // Estudiante y período
        Task<Estudiante?> GetEstudianteByIdAsync(int estudianteId);
        Task<Periodo?> GetPeriodoByIdAsync(int periodoId);

        // Inscripciones
        Task<List<Inscripcion>> GetInscripcionesAsync(int estudianteId, int periodoId);

        // Calificaciones
        Task<List<Calificacion>> GetCalificacionesPorPeriodoAsync(int periodoId);

        // Asistencias
        Task<List<Asistencia>> GetAsistenciasPorFechaAsync(DateTime fecha);

        // Estadísticas
        Task<int> GetTotalEstudiantesAsync();
        Task<int> GetEstudiantesActivosAsync();
        Task<int> GetTotalProfesoresAsync();
        Task<int> GetTotalMateriasAsync();
        Task<int> GetTotalInscripcionesAsync();
        Task<int> GetPeriodosActivosAsync();
        Task<decimal> GetPromedioGeneralCalificacionesAsync();

        // Historial académico
        Task<List<Inscripcion>> GetHistorialEstudianteAsync(int estudianteId);

        // Estudiantes con calificaciones
        Task<List<Estudiante>> GetEstudiantesConCalificacionesPorPeriodoAsync(int periodoId);
    }
}
