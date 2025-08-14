using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Asistencia
{
    public class AsistenciaReadDto
    {
        public int AsistenciaID { get; set; }
        public int InscripcionID { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public bool Asistio { get; set; }
        public string EstadoAsistencia => Asistio ? "Presente" : "Ausente";
        public DateTime FechaCreacion { get; set; }
    }
}
