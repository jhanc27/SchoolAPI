using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Periodo
{
    public class PeriodoCreateDto
    {
        [Required(ErrorMessage = "El nombre del período es requerido")]
        [MaxLength(20, ErrorMessage = "El nombre del período no puede exceder 20 caracteres")]
        public string NombrePeriodo { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;
    }
}
