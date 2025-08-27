using Application.DTOs.Calificacion;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ICalificacionService
    {
      
        
           Task<IEnumerable<CalificacionReadDto>> GetCalificacionesByInscripcionAsync(int inscripcionId);
           Task<IEnumerable<CalificacionReadDto>> GetCalificacionesByEstudianteAsync(int estudianteId);
           Task<CalificacionReadDto?> GetCalificacionByIdAsync(int id);
           Task<CalificacionReadDto> CreateCalificacionAsync(CalificacionCreateDto calificacionDto);
           Task<bool> UpdateCalificacionAsync(int id, CalificacionUpdateDto calificacionDto);
           Task<bool> DeleteCalificacionAsync(int id);

           // Métodos de lógica de negocio
           LiteralCalificacion CalcularLiteralPorNota(decimal nota);
           decimal CalcularNotaPorLiteral(LiteralCalificacion literal);
        
    }
}
