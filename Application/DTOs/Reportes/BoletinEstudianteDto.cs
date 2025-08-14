using Application.DTOs.Estudiante;
using Application.DTOs.Periodo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Reportes
{
    public class BoletinEstudianteDto
    {
        public EstudianteReadDto Estudiante { get; set; } = new();
        public PeriodoReadDto Periodo { get; set; } = new();
        public List<MateriaConCalificacionesDto> Materias { get; set; } = new();
        public decimal PromedioGeneral { get; set; }
    }
}
