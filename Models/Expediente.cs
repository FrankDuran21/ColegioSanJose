using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ColegioSanJose.Models
{
    public class Expediente
    {
        [Key]
        public int ExpedienteId { get; set; }

        [Required]
        public int AlumnoId { get; set; }

        [ForeignKey("AlumnoId")]
        public Alumno? Alumno { get; set; }

        [Required]
        public int MateriaId { get; set; }

        [ForeignKey("MateriaId")]
        public Materia? Materia { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal NotaFinal { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}