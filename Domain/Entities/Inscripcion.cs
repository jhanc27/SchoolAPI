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
    [Index(nameof(EstudianteID), nameof(MateriaID), nameof(PeriodoID), IsUnique = true)]
    [Index(nameof(EstudianteID), nameof(PeriodoID))]
    [Index(nameof(PeriodoID), nameof(MateriaID))]
    public class Inscripcion : IAuditable
    {
        [Key]
        public int InscripcionID { get; set; }

        public required int EstudianteID { get; set; }

        public required int MateriaID { get; set; }

        public required int PeriodoID { get; set; }

        public DateTime FechaCreacion { get; set; }

        // Navegación
        [ForeignKey("EstudianteID")]
        public virtual Estudiante Estudiante { get; set; } = null!;

        [ForeignKey("MateriaID")]
        public virtual Materia Materia { get; set; } = null!;

        [ForeignKey("PeriodoID")]
        public virtual Periodo Periodo { get; set; } = null!;

        public virtual ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
        public virtual ICollection<Asistencia> Asistencias { get; set; } = new List<Asistencia>();
    }
}
