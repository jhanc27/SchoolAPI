using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Calificacion
{
    public class CalificacionReadDto
    {
        public int CalificacionID { get; set; }
        public int InscripcionID { get; set; }
        public string NombreEstudiante { get; set; } = string.Empty;
        public string NombreMateria { get; set; } = string.Empty;
        public string Literal { get; set; } = string.Empty;
        public decimal? Nota { get; set; }

        public string LiteralTexto => Literal switch
        {
            "A" => "Excelente",
            "B" => "Bueno",
            "C" => "Regular",
            "F" => "Reprobado",
            _ => Literal
        };
        public DateTime FechaCreacion { get; set; }
    }
}
