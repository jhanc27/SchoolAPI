using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class HistorialAcademicoDto
    {
        public string Periodo { get; set; } = string.Empty;
        public string Materia { get; set; } = string.Empty;
        public decimal PromedioNotas { get; set; }
        public int TotalCalificaciones { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
        public int TotalClases { get; set; }
    }
}
