using System.ComponentModel.DataAnnotations;

namespace ColegioSanJose.Models
{
    public class Carrera
    {
        [Key]
        public int CarreraId { get; set; }

        [Required(ErrorMessage = "El nombre de la carrera es obligatorio.")]
        [StringLength(100)]
        public string NombreCarrera { get; set; } = string.Empty;
    }
}