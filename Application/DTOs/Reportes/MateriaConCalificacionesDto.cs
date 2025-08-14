using Application.DTOs.Calificacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class MateriaConCalificacionesDto
    {
        public string Materia { get; set; } = string.Empty;
        public string Profesor { get; set; } = string.Empty;
        public List<CalificacionReadDto> Calificaciones { get; set; } = new();
        public decimal PromedioMateria { get; set; }
        public int TotalCalificaciones { get; set; }
    }
}
