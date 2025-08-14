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
    [Index(nameof(ProfesorID))]
    public class Materia : IAuditable
    {
        [Key]
        public int MateriaID { get; set; }

        [MaxLength(100)]
        public required string Nombre { get; set; }

        public required int ProfesorID { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("ProfesorID")]
        public virtual Profesor Profesor { get; set; } = null!;
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
