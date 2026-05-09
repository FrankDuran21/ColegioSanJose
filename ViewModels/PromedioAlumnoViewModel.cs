namespace ColegioSanJose.ViewModels
{
    public class PromedioAlumnoViewModel
    {
        public int AlumnoId { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Grado { get; set; } = string.Empty;

        public decimal PromedioNotas { get; set; }

        public int CantidadMaterias { get; set; }
    }
}