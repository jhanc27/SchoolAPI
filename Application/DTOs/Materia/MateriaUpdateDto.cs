using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Materia
{
    public class MateriaUpdateDto
    {
        [Required(ErrorMessage = "El nombre de la materia es requerido")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El profesor es requerido")]
        public int ProfesorID { get; set; }
    }
}
