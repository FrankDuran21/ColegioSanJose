using System.ComponentModel.DataAnnotations;

namespace ColegioSanJose.Models
{
    public class Alumno
    {
        [Key]
        public int AlumnoId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(50)]
        public string Grado { get; set; } = string.Empty;

        public ICollection<Expediente> Expedientes { get; set; } = new List<Expediente>();
    }
}
