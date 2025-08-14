using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class DetalleAsistenciaDto
    {
        public string Estudiante { get; set; } = string.Empty;
        public string Matricula { get; set; } = string.Empty;
        public string Asistio { get; set; } = string.Empty;
    }
}
