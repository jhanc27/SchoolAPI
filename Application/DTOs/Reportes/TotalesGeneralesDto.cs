using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class TotalesGeneralesDto
    {
        public int TotalMaterias { get; set; }
        public int TotalEstudiantes { get; set; }
        public decimal PromedioGeneral { get; set; }
    }
}
