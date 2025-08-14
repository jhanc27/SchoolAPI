using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Periodo : IAuditable
    {
        [Key]
        public int PeriodoID { get; set; }

        [MaxLength(20)]
        public required string NombrePeriodo { get; set; }

        public required DateTime FechaInicio { get; set; }

        public required DateTime FechaFin { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; }

        // Navegación
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}

