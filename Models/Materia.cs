using System.ComponentModel.DataAnnotations;

namespace ColegioSanJose.Models
{
    public class Materia
    {
        [Key]
        public int MateriaId { get; set; }

        [Required(ErrorMessage = "El nombre de la materia es obligatorio.")]
        [StringLength(100)]
        public string NombreMateria { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre del profesor es obligatorio.")]
        [StringLength(100)]
        public string Docente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El grado es obligatorio.")]
        [StringLength(100)]
        public string Grado { get; set; } = string.Empty;

        public ICollection<Expediente> Expedientes { get; set; } = new List<Expediente>();
    }
}