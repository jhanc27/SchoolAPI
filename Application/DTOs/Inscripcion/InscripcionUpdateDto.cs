using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Inscripcion
{
    public class InscripcionUpdateDto
    {
        [Required(ErrorMessage = "El estudiante es requerido")]
        public int EstudianteID { get; set; }

        [Required(ErrorMessage = "La materia es requerida")]
        public int MateriaID { get; set; }

        [Required(ErrorMessage = "El período es requerido")]
        public int PeriodoID { get; set; }
    }
}
