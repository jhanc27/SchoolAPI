using Application.DTOs.Calificacion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class EstudianteConCalificacionesDto
    {
        public int EstudianteID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public List<CalificacionReadDto> Calificaciones { get; set; } = new();
    }
}
