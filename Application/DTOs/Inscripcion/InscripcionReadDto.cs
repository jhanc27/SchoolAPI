using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inscripcion
{
    public class InscripcionReadDto
    {
        public int InscripcionID { get; set; }
        public int EstudianteID { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string MatriculaEstudiante { get; set; } = string.Empty;
        public int MateriaID { get; set; }
        public string NombreMateria { get; set; } = string.Empty;
        public int PeriodoID { get; set; }
        public string NombrePeriodo { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
