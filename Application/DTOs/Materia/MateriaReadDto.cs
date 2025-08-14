using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Materia
{
    public class MateriaReadDto
    {
        public int MateriaID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int ProfesorID { get; set; }
        public string NombreProfesor { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
