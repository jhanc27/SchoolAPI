using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Calificacion
{
    public class CalificacionCreateDto
    {
        [Required(ErrorMessage = "La inscripción es requerida")]
        public int InscripcionID { get; set; }


        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100")]
        public decimal Nota { get; set; }
    }
}
