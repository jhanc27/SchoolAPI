using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ResumenMateriaDto
    {
        public string Materia { get; set; } = string.Empty;
        public int TotalEstudiantes { get; set; }
        public int TotalCalificaciones { get; set; }
        public decimal PromedioNota { get; set; }
        public decimal NotaMaxima { get; set; }
        public decimal NotaMinima { get; set; }
        public int LiteralesA { get; set; }
        public int LiteralesB { get; set; }
        public int LiteralesC { get; set; }
        public int LiteralesF { get; set; }
        public decimal PorcentajeAprobados { get; set; }
    }
}
