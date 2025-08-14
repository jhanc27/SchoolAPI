using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    [Index(nameof(Matricula), IsUnique = true)]
    public class Estudiante : IAuditable
    {
        [Key]
        public int EstudianteID { get; set; }

        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(100)]
        public required string Apellido { get; set; }

        [MaxLength(20)]
        public required string Matricula { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Inscripcion> Inscripciones { get; set; } = new List<Inscripcion>();
    }
}
