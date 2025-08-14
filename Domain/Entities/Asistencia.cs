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
    [Index(nameof(InscripcionID), nameof(Fecha))]
    public class Asistencia : IAuditable
    {
        [Key]
        public int AsistenciaID { get; set; }

        public required int InscripcionID { get; set; }

        public required DateTime Fecha { get; set; }

        public required bool Asistio { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Navegación
        [ForeignKey("InscripcionID")]
        public virtual Inscripcion Inscripcion { get; set; } = null!;
    }
}
