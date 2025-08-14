using Application.DTOs.Estudiante;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class RendimientoEstudianteDto
    {
        public EstudianteReadDto Estudiante { get; set; } = new();
        public DateTime FechaGeneracion { get; set; }
        public List<HistorialAcademicoDto> HistorialAcademico { get; set; } = new();
        public ResumenRendimientoDto? ResumenGeneral { get; set; }
    }
}
