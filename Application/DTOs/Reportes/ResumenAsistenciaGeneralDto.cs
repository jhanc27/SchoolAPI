using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ResumenAsistenciaGeneralDto
    {
        public int TotalMaterias { get; set; }
        public int TotalEstudiantes { get; set; }
        public int TotalAsistieron { get; set; }
        public int TotalFaltaron { get; set; }
        public decimal PorcentajeAsistenciaGeneral { get; set; }
    }
}
