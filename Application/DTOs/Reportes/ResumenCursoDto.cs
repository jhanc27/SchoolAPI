using Application.DTOs.Periodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ResumenCursoDto
    {
        public PeriodoReadDto Periodo { get; set; } = new();
        public DateTime FechaGeneracion { get; set; }
        public List<ResumenMateriaDto> ResumenPorMateria { get; set; } = new();
        public TotalesGeneralesDto TotalesGenerales { get; set; } = new();
    }
}
