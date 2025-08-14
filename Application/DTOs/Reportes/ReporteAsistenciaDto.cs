using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ReporteAsistenciaDto
    {
        public int EstudianteID { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public int TotalClases { get; set; }
        public int ClasesAsistidas { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
    }
}
