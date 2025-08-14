using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Estudiante
{
    public class EstudianteReadDto
    {
        public int EstudianteID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        // Propiedad derivada
        public string NombreCompleto => $"{Nombre} {Apellido}";

        public string Matricula { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public bool Activo { get; set; }
    }
}
