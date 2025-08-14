using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ReporteAsistenciaDiariaDto
    {
        public string Fecha { get; set; } = string.Empty;
        public DateTime FechaGeneracion { get; set; }
        public List<AsistenciaPorMateriaDto> ReportePorMateria { get; set; } = new();
        public ResumenAsistenciaGeneralDto ResumenGeneral { get; set; } = new();
    }
}
