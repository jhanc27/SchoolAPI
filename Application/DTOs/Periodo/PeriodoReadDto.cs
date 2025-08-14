using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Periodo
{
    public class PeriodoReadDto
    {
        public int PeriodoID { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // Propiedad derivada
        public int DuracionDias => (FechaFin - FechaInicio).Days;
    }
}
