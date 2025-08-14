using Application.DTOs.Materia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class ProfesorConMateriasDto
    {
        public int ProfesorID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public List<MateriaReadDto> Materias { get; set; } = new();
    }
}
