using System.ComponentModel.DataAnnotations;

namespace ColegioSanJose.Models
{
    public class Materia
    {
        [Key]
        public int MateriaId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreMateria { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Docente { get; set; } = string.Empty;

        public ICollection<Expediente> Expedientes { get; set; } = new List<Expediente>();
    }
}