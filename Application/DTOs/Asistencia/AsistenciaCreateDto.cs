using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Asistencia
{
    public class AsistenciaCreateDto
    {
        [Required(ErrorMessage = "La inscripción es requerida")]
        public int InscripcionID { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El estado de asistencia es requerido")]
        public bool Asistio { get; set; }
    }
}
