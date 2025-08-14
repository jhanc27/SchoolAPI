using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Profesor
{
    public class ProfesorReadDto
    {
        public int ProfesorID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string Correo { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
