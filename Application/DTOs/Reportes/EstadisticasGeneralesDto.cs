using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class EstadisticasGeneralesDto
    {
        public int TotalEstudiantes { get; set; }
        public int EstudiantesActivos { get; set; }
        public int TotalProfesores { get; set; }
        public int TotalMaterias { get; set; }
        public int TotalInscripciones { get; set; }
        public int PeriodosActivos { get; set; }
        public decimal PromedioGeneralSistema { get; set; }
        public decimal PorcentajeAsistenciaGeneral { get; set; }
    }
}
