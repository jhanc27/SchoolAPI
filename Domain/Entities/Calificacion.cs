using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public enum LiteralCalificacion { A, B, C, F }
    [Index(nameof(InscripcionID))]
    public class Calificacion : IAuditable
    {
        [Key]
        public int CalificacionID { get; set; }

        public required int InscripcionID { get; set; }

        public required LiteralCalificacion Literal { get; set; }

        [Range(0, 100, ErrorMessage = "La nota debe estar entre 0 y 100")]
        public decimal? Nota { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Navegación
        [ForeignKey("InscripcionID")]
        public virtual Inscripcion Inscripcion { get; set; } = null!;
    }
}
