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
    [Index(nameof(Correo), IsUnique = true)]
    public class Profesor : IAuditable
    {
        [Key]
        public int ProfesorID { get; set; }

        [MaxLength(100)]
        public required string Nombre { get; set; }

        [MaxLength(100)]
        public required string Apellido { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public required string Correo { get; set; }

        [MaxLength(255)]
        public required string Clave { get; set; }

        public DateTime FechaCreacion { get; set; }

        public bool Activo { get; set; } = true;

        // Navegación
        public virtual ICollection<Materia> Materias { get; set; } = new List<Materia>();

    }
}
