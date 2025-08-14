using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ResumenRendimientoDto
    {
        public decimal PromedioGeneral { get; set; }
        public decimal AsistenciaPromedio { get; set; }
        public int TotalMaterias { get; set; }
    }
}
