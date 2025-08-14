using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class AsistenciaPorMateriaDto
    {
        public string Materia { get; set; } = string.Empty;
        public int TotalEstudiantes { get; set; }
        public int Asistieron { get; set; }
        public int Faltaron { get; set; }
        public decimal PorcentajeAsistencia { get; set; }
        public List<DetalleAsistenciaDto> Detalles { get; set; } = new();
    }
}
